FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Define build-time args (defaults empty)
ARG WRITE_DB
ARG JWT_SECRET
ARG JWT_ISSUER
ARG JWT_AUDIENCE
ARG JWT_LIFETIME_MINUTES

# Map them to env vars that ASP.NET Core will pick up
ENV ConnectionStrings__WRITE_DB=${WRITE_DB}
ENV JwtSettings__SecretKey=${JWT_SECRET}
ENV JwtSettings__Issuer=${JWT_ISSUER}
ENV JwtSettings__Audience=${JWT_AUDIENCE}
ENV JwtSettings__TokenLifetimeMinutes=${JWT_LIFETIME_MINUTES}

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
