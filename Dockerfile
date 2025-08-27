# 🔹 Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia apenas o csproj e restaura dependências
COPY ProfessorApp.Api/ProfessorApp.Api.csproj ./ProfessorApp.Api.csproj
RUN dotnet restore ./ProfessorApp.Api.csproj

# Copia todo o código
COPY . ./
RUN dotnet publish ./ProfessorApp.Api.csproj -c Release -o /app/out

# 🔹 Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

EXPOSE 5000
ENTRYPOINT ["dotnet", "ProfessorApp.Api.dll"]
