# Usando imagem base do SDK para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia arquivos e restaura dependências
COPY *.csproj ./
RUN dotnet restore

# Copia tudo e builda
COPY . ./
RUN dotnet publish -c Release -o out

# Usando imagem menor do runtime para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expõe a porta usada pela aplicação
EXPOSE 5000

# Comando para rodar a aplicação
ENTRYPOINT ["dotnet", "ProfessorApp.Api.dll"]
