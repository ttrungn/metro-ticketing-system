FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CatalogService/Directory.Packages.props", "CatalogService/"]
COPY ["CatalogService/Directory.Build.props", "CatalogService/"]
COPY ["CatalogService/global.json", "CatalogService/"]
COPY ["CatalogService/src/Web/Web.csproj", "CatalogService/src/Web/"]
COPY ["CatalogService/src/Application/Application.csproj", "CatalogService/src/Application/"]
COPY ["CatalogService/src/Domain/Domain.csproj", "CatalogService/src/Domain/"]
COPY ["BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/"]
COPY ["CatalogService/src/Infrastructure/Infrastructure.csproj", "CatalogService/src/Infrastructure/"]
RUN dotnet restore "CatalogService/src/Web/Web.csproj"
COPY . .
WORKDIR "/src/CatalogService/src/Web"
RUN dotnet build "./Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CatalogService.Web.dll"]
