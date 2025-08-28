# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia arquivos do csproj e restaura dependências
COPY *.csproj ./
RUN dotnet restore

# Copia todo o restante do código e publica
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Exposição e execução
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "ProfessorApp.Api.dll"]
