FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/OrderHub.Contracts/OrderHub.Contracts.csproj src/OrderHub.Contracts/
COPY src/InventoryService/InventoryService.csproj src/InventoryService/
RUN dotnet restore src/InventoryService/InventoryService.csproj
COPY src/OrderHub.Contracts/ src/OrderHub.Contracts/
COPY src/InventoryService/ src/InventoryService/
RUN dotnet publish src/InventoryService/InventoryService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
ENV ASPNETCORE_HTTP_PORTS=5003
EXPOSE 5003
HEALTHCHECK CMD curl -f http://localhost:5003/health || exit 1
ENTRYPOINT ["dotnet", "InventoryService.dll"]
