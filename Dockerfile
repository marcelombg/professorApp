# 🔹 Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia os arquivos de projeto e restaura dependências
COPY ProfessorApp.Api/*.csproj ./ProfessorApp.Api/
RUN dotnet restore ./ProfessorApp.Api/ProfessorApp.Api.csproj

# Copia todo o código e publica
COPY . ./
RUN dotnet publish ./ProfessorApp.Api/ProfessorApp.Api.csproj -c Release -o /app/out

# 🔹 Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copia os arquivos publicados do estágio de build
COPY --from=build /app/out ./

# Expõe a porta
EXPOSE 5000

# Comando para rodar a aplicação
ENTRYPOINT ["dotnet", "ProfessorApp.Api.dll"]
