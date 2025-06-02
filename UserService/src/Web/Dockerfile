FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UserService/Directory.Packages.props", "UserService/"]
COPY ["UserService/Directory.Build.props", "UserService/"]
COPY ["UserService/global.json", "UserService/"]
COPY ["UserService/src/Web/Web.csproj", "UserService/src/Web/"]
COPY ["UserService/src/Application/Application.csproj", "UserService/src/Application/"]
COPY ["UserService/src/Domain/Domain.csproj", "UserService/src/Domain/"]
COPY ["BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/"]
COPY ["UserService/src/Infrastructure/Infrastructure.csproj", "UserService/src/Infrastructure/"]
RUN dotnet restore "UserService/src/Web/Web.csproj"
COPY . .
WORKDIR "/src/UserService/src/Web"
RUN dotnet build "./Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.Web.dll"]
