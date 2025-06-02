FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OrderService/Directory.Packages.props", "OrderService/"]
COPY ["OrderService/Directory.Build.props", "OrderService/"]
COPY ["OrderService/global.json", "OrderService/"]
COPY ["OrderService/src/Web/Web.csproj", "OrderService/src/Web/"]
COPY ["OrderService/src/Application/Application.csproj", "OrderService/src/Application/"]
COPY ["OrderService/src/Domain/Domain.csproj", "OrderService/src/Domain/"]
COPY ["BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/"]
COPY ["OrderService/src/Infrastructure/Infrastructure.csproj", "OrderService/src/Infrastructure/"]
RUN dotnet restore "OrderService/src/Web/Web.csproj"
COPY . .
WORKDIR "/src/OrderService/src/Web"
RUN dotnet build "./Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.Web.dll"]
