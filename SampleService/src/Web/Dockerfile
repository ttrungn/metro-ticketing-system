FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ARG WRITE_DB
ARG READ_DB
ARG KAFKA_URL
ARG KAFKA_USERNAME
ARG KAFKA_PASSWORD
ARG KAFKA_SAMPLE_EVENTS_NAME
ARG KAFKA_SAMPLE_EVENTS_GROUP
ARG KAFKA_PRODUCER_MESSAGE_TIMEOUT_MS
ARG KAFKA_PRODUCER_MAX_RETRIES
ARG KAFKA_PRODUCER_BACKOFF_MS

ENV ConnectionStrings__WriteDb=${WRITE_DB}
ENV ConnectionStrings__ReadDb=${READ_DB}
ENV KafkaSettings__Url=${KAFKA_URL}
ENV KafkaSettings__Username=${KAFKA_USERNAME}
ENV KafkaSettings__Password=${KAFKA_PASSWORD}
ENV KafkaSettings__SampleEvents__Name=${KAFKA_SAMPLE_EVENTS_NAME}
ENV KafkaSettings__SampleEvents__Group=${KAFKA_SAMPLE_EVENTS_GROUP}
ENV KafkaSettings__ProducerConfigs__MessageTimeoutMs=${KAFKA_PRODUCER_MESSAGE_TIMEOUT_MS}
ENV KafkaSettings__ProducerConfigs__MessageSendMaxRetries=${KAFKA_PRODUCER_MAX_RETRIES}
ENV KafkaSettings__ProducerConfigs__RetryBackoffMs=${KAFKA_PRODUCER_BACKOFF_MS}

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
