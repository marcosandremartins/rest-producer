FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
ARG RELEASE_VERSION

WORKDIR /source
COPY ["src/KafkaRestProducer.sln", "./"]
COPY ["src/", "."]

RUN dotnet restore KafkaRestProducer.sln -p:Version=${RELEASE_VERSION}
RUN dotnet build KafkaRestProducer.sln --configuration Release --no-restore -p:Version=${RELEASE_VERSION}
RUN dotnet publish KafkaRestProducer/KafkaRestProducer.csproj --configuration Release --runtime linux-x64 --no-build -p:Version=${RELEASE_VERSION} -o publish

FROM mcr.microsoft.com/dotnet/sdk:6.0 as sdkfinal
ARG SERVICE_PORT=5001
ENV ASPNETCORE_URLS=http://+:${SERVICE_PORT}
EXPOSE ${SERVICE_PORT}

WORKDIR /app
RUN mkdir contracts
COPY --from=build /source/publish .

FROM mcr.microsoft.com/dotnet/runtime:6.0 as final
ARG SERVICE_PORT=5001
ENV ASPNETCORE_URLS=http://+:${SERVICE_PORT}
EXPOSE ${SERVICE_PORT}

WORKDIR /app
RUN mkdir contracts
COPY --from=build /source/publish .

ENTRYPOINT ["dotnet", "KafkaRestProducer.dll"]
