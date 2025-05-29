#!/bin/bash

# --- Configurações ---
DOCKER_COMPOSE_FILE="docker-compose.yml"
KAFKA_CONNECT_HOST="localhost"
KAFKA_CONNECT_PORT="8083"
CONNECTORS_API_URL="http://${KAFKA_CONNECT_HOST}:${KAFKA_CONNECT_PORT}/connectors"
MAX_RETRIES=60 # Número máximo de tentativas para verificar serviços (aprox. 5 minutos com 5s de delay)
RETRY_DELAY=5  # Atraso entre as tentativas em segundos

# --- Credenciais do PostgreSQL (para verificação de prontidão e migração) ---
# ATENÇÃO: Para produção, use variáveis de ambiente seguras!
DB_HOST="localhost" # Usamos localhost aqui porque estamos acessando o Postgres de fora do container via porta mapeada
DB_PORT="5432"
DB_USER="postgres"
DB_NAME="ecommerce"
DB_PASSWORD="password"

# --- Verificação de Pré-requisitos ---

# Verifica se o 'curl' está instalado
if ! command -v curl &> /dev/null; then
    echo "Erro: O comando 'curl' não foi encontrado."
    echo "Por favor, instale o 'curl' no seu ambiente (Git Bash no Windows, ou 'sudo apt install curl' no WSL/Linux)."
    echo "Encerrando o script."
    exit 1
fi

# Verifica se o 'dotnet' está instalado
if ! command -v dotnet &> /dev/null; then
    echo "Erro: O SDK do .NET não foi encontrado."
    echo "Por favor, instale o SDK do .NET 9 (ou versão compatível) em sua máquina."
    echo "Encerrando o script."
    exit 1
fi

# Verifica se o 'dotnet ef' (Entity Framework Core CLI) está instalado
if ! dotnet ef &> /dev/null; then
    echo "Erro: A ferramenta 'dotnet ef' não foi encontrada."
    echo "Por favor, instale a ferramenta 'dotnet ef' globalmente: 'dotnet tool install --global dotnet-ef'"
    echo "Encerrando o script."
    exit 1
fi

# --- Funções Auxiliares ---

# Função para aguardar o serviço Kafka Connect estar ativo
wait_for_kafka_connect() {
  echo "Aguardando o Kafka Connect estar ativo em ${CONNECTORS_API_URL}..."
  for i in $(seq 1 $MAX_RETRIES); do
    STATUS_CODE=$(curl -s -o /dev/null -w "%{http_code}" "${CONNECTORS_API_URL}")
    if [ "$STATUS_CODE" -eq 200 ]; then
      echo "Kafka Connect está ativo! (HTTP 200)"
      return 0
    else
      echo "Tentativa $i de $MAX_RETRIES: Kafka Connect não está pronto (HTTP $STATUS_CODE). Aguardando ${RETRY_DELAY}s..."
      sleep $RETRY_DELAY
    fi
  done

  echo "Erro: Kafka Connect não ficou ativo após $MAX_RETRIES tentativas. Encerrando."
  exit 1
}

# --- Execução Principal ---

echo "Iniciando os serviços Docker Compose..."
# Subir os serviços em segundo plano, mas sem o 'app' se você pretende rodá-lo localmente depois.
# Se o 'app' estiver no docker-compose.yml e você quiser que ele suba no final,
# apenas remova ele das dependências do start.sh e do docker-compose.yml durante o teste
# ou tenha certeza que ele não tentará conectar ao DB antes das migrations.
# Para este script, assume-se que 'app' pode iniciar com DB não migrado, ou que você o subirá manualmente depois.
docker compose -f "${DOCKER_COMPOSE_FILE}" up -d

# 1. Aguardar o PostgreSQL estar pronto antes de rodar migrações
# wait_for_postgres

# 2. Executar as migrações do Entity Framework Core localmente
echo "Executando as migrações do Entity Framework Core localmente..."
# O caminho '--project src/Api/Api.csproj' é relativo à raiz do projeto, onde o script está sendo executado.
# Use --startup-project para garantir que o projeto correto seja usado para resolver dependências.
dotnet ef database update --project src/Api/Api.csproj --startup-project src/Api/Api.csproj

# Verifica o código de saída do comando EF
if [ $? -ne 0 ]; then
    echo "Erro: As migrações do Entity Framework Core falharam. Encerrando."
    echo "Certifique-se de que o SDK .NET e 'dotnet ef' estão instalados e configurados corretamente."
    exit 1
fi
echo "Migrações do Entity Framework Core aplicadas com sucesso!"


# 3. Aguardar o Kafka Connect estar ativo para configurar conectores
wait_for_kafka_connect

echo "Kafka Connect ativo. Prosseguindo com a configuração dos conectores via curl..."

# --- 4. Criar o Debezium PostgreSQL Connector ---
echo "Criando o conector Debezium (postgres-connector)..."
curl --location "${CONNECTORS_API_URL}" \
--header 'Content-Type: application/json' \
--data '{
    "name": "postgres-connector",
    "config": {
        "connector.class": "io.debezium.connector.postgresql.PostgresConnector",
        "database.hostname": "postgres",
        "database.port": "5432",
        "database.user": "postgres",
        "database.password": "password",
        "database.dbname": "ecommerce",
        "database.server.name": "dbecommerce",
        "table.include.list": "public.products",
        "plugin.name": "pgoutput",
        "topic.prefix": "dbecommerce",
        "decimal.handling.mode": "double",
        "key.converter": "org.apache.kafka.connect.json.JsonConverter",
        "key.converter.schemas.enable": "false",
        "value.converter": "org.apache.kafka.connect.json.JsonConverter",
        "value.converter.schemas.enable": "false",
        "transforms": "unwrap",
        "transforms.unwrap.type": "io.debezium.transforms.ExtractNewRecordState",
        "transforms.unwrap.drop.tombstones": "true",
        "transforms.unwrap.delete.handling.mode": "none"
    }
}'
echo "" # Adiciona uma nova linha para melhor legibilidade após o curl

# --- 5. Criar o Elasticsearch Sink Connector ---
echo "Criando o conector Elasticsearch Sink (es-sink-connector)..."
curl --location "${CONNECTORS_API_URL}" \
--header 'Content-Type: application/json' \
--data '{
    "name": "es-sink-connector",
    "config": {
        "connector.class": "io.confluent.connect.elasticsearch.ElasticsearchSinkConnector",
        "tasks.max": "1",
        "topics": "dbecommerce.public.products",
        "connection.url": "http://elasticsearch:9200",
        "type.name": "_doc",
        "key.ignore": "false",
        "schema.ignore": "true",
        "behavior.on.null.values": "delete",
        "drop.invalid.message": true,
        "transforms": "ExtractKey",
        "transforms.ExtractKey.type": "org.apache.kafka.connect.transforms.ExtractField$Key",
        "transforms.ExtractKey.field": "id",
        "key.converter": "org.apache.kafka.connect.json.JsonConverter",
        "key.converter.schemas.enable": false,
        "value.converter": "org.apache.kafka.connect.json.JsonConverter",
        "value.converter.schemas.enable": false
    }
}'
echo "" # Adiciona uma nova linha para melhor legibilidade após o curl

echo "Configuração dos conectores finalizada."
echo "O ambiente Docker Compose está pronto e o banco de dados migrado."
echo "Agora você pode iniciar sua aplicação .NET localmente ('dotnet run' em src/Api/) ou via Docker Compose se ela estiver habilitada."