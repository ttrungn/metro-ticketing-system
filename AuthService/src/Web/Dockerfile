FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AuthService/Directory.Packages.props", "AuthService/"]
COPY ["AuthService/Directory.Build.props", "AuthService/"]
COPY ["AuthService/global.json", "AuthService/"]
COPY ["AuthService/src/Web/Web.csproj", "AuthService/src/Web/"]
COPY ["AuthService/src/Application/Application.csproj", "AuthService/src/Application/"]
COPY ["AuthService/src/Domain/Domain.csproj", "AuthService/src/Domain/"]
COPY ["BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/"]
COPY ["AuthService/src/Infrastructure/Infrastructure.csproj", "AuthService/src/Infrastructure/"]
RUN dotnet restore "AuthService/src/Web/Web.csproj"
COPY . .
WORKDIR "/src/AuthService/src/Web"
RUN dotnet build "./Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthService.Web.dll"]
