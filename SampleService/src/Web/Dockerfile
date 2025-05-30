FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SampleService/Directory.Packages.props", "SampleService/"]
COPY ["SampleService/Directory.Build.props", "SampleService/"]
COPY ["SampleService/global.json", "SampleService/"]
COPY ["SampleService/src/Web/Web.csproj", "SampleService/src/Web/"]
COPY ["SampleService/src/Application/Application.csproj", "SampleService/src/Application/"]
COPY ["SampleService/src/Domain/Domain.csproj", "SampleService/src/Domain/"]
COPY ["BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/"]
COPY ["SampleService/src/Infrastructure/Infrastructure.csproj", "SampleService/src/Infrastructure/"]
RUN dotnet restore "SampleService/src/Web/Web.csproj"
COPY . .
WORKDIR "/src/SampleService/src/Web"
RUN dotnet build "./Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SampleService.Web.dll"]
