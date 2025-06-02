FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NotificationService/Directory.Packages.props", "NotificationService/"]
COPY ["NotificationService/Directory.Build.props", "NotificationService/"]
COPY ["NotificationService/global.json", "NotificationService/"]
COPY ["NotificationService/src/Web/Web.csproj", "NotificationService/src/Web/"]
COPY ["NotificationService/src/Application/Application.csproj", "NotificationService/src/Application/"]
COPY ["NotificationService/src/Domain/Domain.csproj", "NotificationService/src/Domain/"]
COPY ["BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/"]
COPY ["NotificationService/src/Infrastructure/Infrastructure.csproj", "NotificationService/src/Infrastructure/"]
RUN dotnet restore "NotificationService/src/Web/Web.csproj"
COPY . .
WORKDIR "/src/NotificationService/src/Web"
RUN dotnet build "./Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.Web.dll"]
