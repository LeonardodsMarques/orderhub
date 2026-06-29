FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/OrderService/OrderService.Domain/OrderService.Domain.csproj src/OrderService/OrderService.Domain/
COPY src/OrderService/OrderService.Application/OrderService.Application.csproj src/OrderService/OrderService.Application/
COPY src/OrderService/OrderService.Infrastructure/OrderService.Infrastructure.csproj src/OrderService/OrderService.Infrastructure/
COPY src/OrderService/OrderService.API/OrderService.API.csproj src/OrderService/OrderService.API/
RUN dotnet restore src/OrderService/OrderService.API/OrderService.API.csproj
COPY src/OrderService/ src/OrderService/
RUN dotnet publish src/OrderService/OrderService.API/OrderService.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
ENV ASPNETCORE_HTTP_PORTS=5001
EXPOSE 5001
HEALTHCHECK CMD curl -f http://localhost:5001/health || exit 1
ENTRYPOINT ["dotnet", "OrderService.API.dll"]
