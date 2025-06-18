# Usando imagem base do SDK para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia apenas os arquivos de projeto e restaura
COPY ProfessorApp.Api/*.csproj ./ProfessorApp.Api/
WORKDIR /app/ProfessorApp.Api
RUN dotnet restore

# Volta para pasta raiz e copia tudo
WORKDIR /app
COPY . .
WORKDIR /app/ProfessorApp.Api
RUN dotnet publish -c Release -o /app/out

# Usando imagem menor do runtime para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expõe a porta usada pela aplicação
EXPOSE 5000

# Comando para rodar a aplicação
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "ProfessorApp.Api.dll"]
