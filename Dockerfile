FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
ARG RELEASE_VERSION

WORKDIR /source
COPY ["KafkaRestProducer.sln", "."]
COPY ["src/", "src/"]

RUN dotnet restore KafkaRestProducer.sln -p:Version=${RELEASE_VERSION}
RUN dotnet build KafkaRestProducer.sln --configuration Release --no-restore -p:Version=${RELEASE_VERSION}
RUN dotnet publish src/KafkaRestProducer/KafkaRestProducer.csproj --configuration Release --runtime linux-x64 --no-build -p:Version=${RELEASE_VERSION} -o publish

FROM build AS tests
CMD dotnet build KafkaRestProducer.sln --configuration Release --no-restore && \
    dotnet test src/KafkaRestProducer.Tests/KafkaRestProducer.Tests.csproj --configuration Release --no-build && \
    dotnet test src/KafkaRestProducer.IntegrationTests/KafkaRestProducer.IntegrationTests.csproj --configuration Release --no-build

FROM mcr.microsoft.com/dotnet/sdk:6.0 as sdkfinal
ARG SERVICE_PORT=5001
ENV ASPNETCORE_URLS=http://+:${SERVICE_PORT}
EXPOSE ${SERVICE_PORT}

WORKDIR /
RUN wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN rm packages-microsoft-prod.deb
RUN apt update && apt install -y dotnet-runtime-3.1

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
