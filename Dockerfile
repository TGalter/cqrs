# Stage 1: Build a .NET application
# Use a imagem oficial do .NET SDK como base para a etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release # Configuração de build padrão é Release

# Define o diretório de trabalho principal dentro do contêiner como /project
# Todos os caminhos subsequentes serão relativos a este diretório
WORKDIR /project

# Copia o arquivo de solução (ec.sln) da raiz do seu repositório para /project
COPY ["ec.sln", "."]

# Copia todos os arquivos .csproj de cada pasta de projeto para seus respectivos diretórios
# dentro de /project/src/
COPY ["src/App/*.csproj", "src/App/"]
COPY ["src/Dom/*.csproj", "src/Dom/"]
COPY ["src/Infra/*.csproj", "src/Infra/"]
COPY ["src/Api/*.csproj", "src/Api/"]

# Restaura os pacotes NuGet para todos os projetos na solução
RUN dotnet restore "ec.sln"

# Copia todo o restante do código-fonte para o diretório de trabalho /project
COPY . .

# Altera o diretório de trabalho para o diretório do projeto da API dentro de /project
# Este é o projeto que será publicado
WORKDIR /project/src/Api

# Publica a aplicação para implantação
# O caminho de saída é /app/publish dentro do contêiner
RUN dotnet publish "Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 2: Create the final runtime image
# Usa a imagem oficial do runtime ASP.NET como base para a imagem final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copia a saída publicada da etapa de build para a imagem final
COPY --from=build /app/publish .

# Expõe a porta que sua aplicação ASP.NET Core escuta (o padrão para contêineres .NET 8+ é 8080)
EXPOSE 8080

# Define o ponto de entrada para o contêiner.
# Este comando executa sua aplicação quando o contêiner inicia.
ENTRYPOINT ["dotnet", "Api.dll"]