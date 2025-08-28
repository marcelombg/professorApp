# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia csproj e restaura dependências
COPY *.sln .
COPY ProfessorApp.Api/*.csproj ./ProfessorApp.Api/
RUN dotnet restore

# Copia todo o código
COPY . .

# Build do projeto
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Variável de ambiente DATABASE_URL será definida no Railway
# Exemplo no Railway: postgresql://user:password@host:port/dbname

# Porta que o ASP.NET irá ouvir
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_URLS=http://+:5000
EXPOSE 5000

# Comando para rodar a aplicação
ENTRYPOINT ["dotnet", "ProfessorApp.Api.dll"]
