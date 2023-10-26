#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/Home.Api/Home.Api.csproj", "Services/Home.Api/"]
COPY ["BuildingBlocks/EventBus.IoC/EventBus.IoC.csproj", "BuildingBlocks/EventBus.IoC/"]
COPY ["BuildingBlocks/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "BuildingBlocks/EventBusRabbitMQ/"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/"]
COPY ["BuildingBlocks/IntegrationEventLog/IntegrationEventLog.csproj", "BuildingBlocks/IntegrationEventLog/"]
RUN dotnet restore "Services/Home.Api/Home.Api.csproj"
COPY . .
WORKDIR "/src/Services/Home.Api"
RUN dotnet build "Home.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Home.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Home.Api.dll"]