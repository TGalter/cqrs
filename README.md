# 🚀 CQRS Template

Este projeto é um template, construída com uma arquitetura **CQRS (Command Query Responsibility Segregation)**, utilizando **.NET 9** e uma stack de tecnologias robustas para garantir escalabilidade, resiliência e alta disponibilidade. O foco principal está na separação explícita entre operações de escrita (Comandos) e leitura (Consultas), otimizando o fluxo de dados e o desempenho.

---

## 🚧 Status do Projeto

Este projeto está **em desenvolvimento ativo**. Funcionalidades básicas de domínio e persistência estão em progresso, e o pipeline de Change Data Capture (CDC) para indexação no Elasticsearch está estabelecido. Novas funcionalidades e otimizações serão adicionadas continuamente.

---

## 🛠️ Tecnologias Utilizadas

A seguir, uma lista detalhada das principais tecnologias e ferramentas empregadas neste projeto:

### Backend & Core
* **`.NET 9` (C#):** Framework principal para o desenvolvimento da API e serviços backend.
* **`CQRS (Command Query Responsibility Segregation)`:** Padrão arquitetural que segrega as operações de leitura e escrita, resultando em modelos de dados otimizados para cada finalidade.
* **`MediatR`:** Biblioteca para implementação do padrão Mediator, facilitando a comunicação entre comandos/queries e seus respectivos handlers.
* **`Entity Framework Core (EF Core)`:** ORM para interação com o banco de dados relacional.

### Banco de Dados & Persistência
* **`PostgreSQL`:** Banco de dados relacional robusto e escalável para persistência transacional dos dados do domínio.
    * **`Npgsql.EntityFrameworkCore.PostgreSQL`:** Provider para EF Core interagir com PostgreSQL.

### Mensageria & Event Streaming
* **`Apache Kafka`:** Plataforma distribuída de streaming de eventos, utilizada para garantir a consistência eventual e a comunicação assíncrona entre os componentes.
* **`Confluent.Kafka`:** Cliente .NET para Apache Kafka.
* **`Kafka Connect`:** Framework para conectar Kafka com outros sistemas, facilitando a ingestão e exportação de dados.
    * **`Debezium`:** Conector do Kafka Connect para **Change Data Capture (CDC)**, monitorando o PostgreSQL e publicando as alterações no Kafka.
    * **`Elasticsearch Sink Connector`:** Conector do Kafka Connect para consumir eventos do Kafka e indexá-los no Elasticsearch.

### Busca & Análise
* **`Elasticsearch`:** Motor de busca distribuído, utilizado para indexação e consultas em tempo real dos dados, otimizando as operações de leitura (Queries).
    * **`NEST`:** Cliente oficial de alto nível do Elasticsearch para .NET, usado para interagir com o cluster Elasticsearch.

### Orquestração & Desenvolvimento
* **`Docker` & `Docker Compose`:** Utilizados para containerização e orquestração de todos os serviços e bases de dados (PostgreSQL, Kafka, Kafka Connect, Elasticsearch, etc.) em um ambiente de desenvolvimento local e consistente.

---

## 🚀 Arquitetura Geral

A arquitetura do projeto segue o padrão CQRS:

1.  **Comandos (Escrita):**
    * Requisições de escrita (ex: criar produto, atualizar estoque) são modeladas como `Commands`.
    * `Commands` são enviados ao `MediatR` e processados por seus respectivos `Command Handlers`.
    * Os `Command Handlers` interagem com o `Entity Framework Core` para persistir os dados no `PostgreSQL`.

2.  **Queries (Leitura):**
    * Requisições de leitura (ex: buscar produto por ID, listar produtos) são modeladas como `Queries`.
    * `Queries` são enviadas ao `MediatR` e processadas por seus respectivos `Query Handlers`.
    * Os `Query Handlers` realizam consultas otimizadas diretamente no `Elasticsearch` para recuperar os dados rapidamente.

3.  **Sincronização de Dados (CDC):**
    * **`Debezium`** monitora o `PostgreSQL` para capturar todas as alterações (INSERTs, UPDATEs, DELETEs) no banco de dados.
    * Essas alterações são publicadas como eventos no `Apache Kafka`.
    * O **`Elasticsearch Sink Connector`** consome esses eventos do Kafka, aplica transformações (como `ExtractKey` para usar o `id` da chave do Kafka como `_id` no Elasticsearch) e indexa os dados no `Elasticsearch`.

---

## ⚙️ Configuração do Ambiente de Desenvolvimento

Para rodar o projeto, você precisará dos seguintes softwares instalados em sua máquina:

1.  **Docker Desktop:**
    * Inclui o Docker Daemon e o Docker Compose. Essencial para orquestrar os serviços containerizados.
    * Siga a documentação oficial para a instalação: [https://docs.docker.com/desktop/install/](https://docs.docker.com/desktop/install/)

2.  **SDK do .NET 9:**
    * Necessário para compilar e executar a aplicação .NET localmente, e para rodar as migrações do EF Core fora do container.
    * Baixe e instale a versão mais recente do .NET 9 SDK (ou a versão compatível com o projeto) para o seu sistema operacional.
    * Documentação oficial de instalação: [https://learn.microsoft.com/pt-br/dotnet/core/install/](https://learn.microsoft.com/pt-br/dotnet/core/install/)

3.  **Ferramentas do Entity Framework Core CLI (`dotnet ef`):**
    * Essencial para aplicar as migrações do banco de dados a partir da linha de comando.
    * Após instalar o SDK do .NET, instale a ferramenta globalmente:
        ```bash
        dotnet tool install --global dotnet-ef --version 9.*
        ```
    * Se você já tiver uma versão mais antiga, pode atualizar com:
        ```bash
        dotnet tool update --global dotnet-ef --version 9.*
        ```
    * Documentação oficial de instalação das ferramentas do EF Core: [https://learn.microsoft.com/pt-br/ef/core/cli/dotnet](https://learn.microsoft.com/pt-br/ef/core/cli/dotnet)

4.  **Git Bash (para Windows) ou WSL (Windows Subsystem for Linux):**
    * **No Windows, é essencial ter um ambiente Bash compatível para executar o script `start.sh` e seus comandos (como `curl` e `nc`).**
    * **Git Bash:** Baixe e instale o Git for Windows, que inclui o Git Bash: [https://git-scm.com/download/win](https://git-scm.com/download/win)
    * **WSL:** Como alternativa, você pode configurar o WSL (Windows Subsystem for Linux) se preferir um ambiente Linux completo dentro do Windows.

---

## 🚀 Primeiros Passos

1.  **Clone o Repositório:**
    Abra seu terminal Bash (Git Bash no Windows, ou terminal Linux/macOS) e clone o repositório:
    ```bash
    git clone https://github.com/TGalter/cqrs.git
    cd cqrs
    ```

2.  **Execute o Script de Inicialização:**
    * Este projeto inclui um script `start.sh` na raiz do projeto para facilitar a inicialização de todos os serviços Docker e a configuração dos conectores do Kafka Connect, além de aplicar as migrações do banco de dados.
    * **Certifique-se de que todos os pré-requisitos listados acima estão instalados antes de prosseguir.**
    * Dê permissão de execução ao script:
        ```bash
        chmod +x start.sh
        ```
    * E execute:
        ```bash
        ./start.sh
        ```
        Este script irá:
        * Levantar todos os serviços definidos no `docker-compose.yml` em segundo plano.
        * Aguardar que o PostgreSQL esteja totalmente pronto para conexões.
        * **Executar as migrações do Entity Framework Core localmente**, garantindo que o banco de dados esteja com o esquema atualizado.
        * Aguardar que o Kafka Connect esteja totalmente pronto.
        * Configurar os conectores Debezium (PostgreSQL CDC) e Elasticsearch Sink no Kafka Connect.

### Rodando a Aplicação .NET Localmente (fora do Docker)

Após o script `start.sh` ter concluído com sucesso (todos os serviços Docker rodando e o banco de dados migrado), você pode iniciar sua aplicação .NET API em sua máquina local:

1.  **Navegue até o diretório do projeto da API:**
    ```bash
    cd src/Api/
    ```
2.  **Execute a Aplicação:**
    ```bash
    dotnet run
    ```
    * Sua aplicação usará as configurações de `appsettings.Development.json` e se conectará aos serviços Docker através de `localhost` (ex: `Host=localhost:5432` para PostgreSQL, `http://localhost:9200` para Elasticsearch, etc.).

**Nota:** Se você deseja rodar sua aplicação .NET API também dentro do Docker, certifique-se de que o serviço `app` não está comentado no `docker-compose.yml` e que ele aponta para os nomes de serviço internos do Docker (ex: `Host=postgres`). Se você usa `start.sh` para migrar e depois `docker compose up -d`, o `app` iniciará com o DB já migrado.

---

## 🤝 Contribuição

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues para bugs ou sugestões, ou enviar Pull Requests.

---

## 📄 Licença

MIT License

Copyright (c) 2025 Thales Galter

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

---