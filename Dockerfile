# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia o csproj e restaura dependências
COPY *.csproj ./
RUN dotnet restore

# Copia todo o restante do código e publica
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Porta padrão do Railway
ENV ASPNETCORE_URLS=http://+:5000

# Comando para rodar a aplicação
ENTRYPOINT ["dotnet", "ProfessorApp.Api.dll"]

# Expõe a porta usada pelo Railway
EXPOSE 5000
