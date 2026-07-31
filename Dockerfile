FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY *.slnx ./
COPY src/DeportivoUCN.API/*.csproj            ./src/DeportivoUCN.API/
COPY src/DeportivoUCN.Application/*.csproj    ./src/DeportivoUCN.Application/
COPY src/DeportivoUCN.Infrastructure/*.csproj ./src/DeportivoUCN.Infrastructure/
COPY src/DeportivoUCN.Models/*.csproj         ./src/DeportivoUCN.Models/

RUN dotnet restore DeportivoUCN.slnx

COPY . .
WORKDIR /app/src/DeportivoUCN.API

RUN dotnet publish "DeportivoUCN.API.csproj" \
    -c Release \
    -o /app/out \
 && echo "=== Archivos publicados ===" \
 && ls -la /app/out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 8080
ENTRYPOINT ["dotnet", "DeportivoUCN.API.dll"]