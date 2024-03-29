# Kafka REST Producer

Publish any message to Kafka through a RESTFul API

## Requirements

- [Kafka](https://hub.docker.com/r/confluentinc/cp-kafka)
- [Schema Registry](https://hub.docker.com/r/confluentinc/cp-schema-registry)

## DockerHub Images

- [rest-producer](https://hub.docker.com/r/marcosandremartins/rest-producer)
- [rest-producer-with-sdk](https://hub.docker.com/r/marcosandremartins/rest-producer-with-sdk)

## Quickstart

Running slim image off a composer

```yaml
version: '3.4'
services:
  rest-producer:
    image: marcosandremartins/rest-producer:latest
    environment:
      Settings__KafkaBrokers__0: kafka:9092
      Settings__SchemaRegistryUrl: schemaregistry:8081
      # the settings below are optional, they are only necessary 
      # if your schema registry server is using basic authentication 
      Settings__SchemaRegistryAuth__Username: username
      Settings__SchemaRegistryAuth__Password: password
    ports:
      - "5001:5001"
    volumes:
      - ./contracts:/app/contracts
```

Running image with .net6 sdk off a composer

```yaml
version: '3.4'
services:
  rest-producer-with-sdk:
    image: marcosandremartins/rest-producer-with-sdk:latest
    environment:
      Settings__KafkaBrokers__0: kafka:9092
      Settings__SchemaRegistryUrl: schemaregistry:8081
      # the settings below are optional, they are only necessary 
      # if your schema registry server is using basic authentication 
      Settings__SchemaRegistryAuth__Username: username
      Settings__SchemaRegistryAuth__Password: password
    ports:
      - "5001:5001"
    command: >
      bash -c "dotnet nuget add source https://some-local-source -n local-source  -u username -p password
               dotnet new console -n sample
               dotnet add sample/sample.csproj package Package.Name
               dotnet publish sample/sample.csproj -o /app/contracts/
               dotnet KafkaRestProducer.dll"
```

The following assumes you have Kafka, SchemaRegistry and an instance of the REST Producer running with the appropriate contracts assemblies loaded

```bash
    # Produce a JSON based message Compressed with Gzip
    $ curl -X 'POST' \
      'http://localhost:5001/topics' \
      -H 'Content-Type: application/json' \
      -d '{
            "topic": "test-topic",
            "key": "1",
            "serializer": "Json",
            "compressionType": "Gzip",
            "payload": {
              "Name": "some guy",
              "Id": 1
            },
            "headers": {
              "Timestamp": "2022-06-01T12:00:00.001Z"
            }
         }'

    # Produce a JSON based message serialized with Protobuf
    $ curl -X 'POST' \
      'http://localhost:5001/topics' \
      -H 'Content-Type: application/json' \
      -d '{
            "topic": "test-topic",
            "key": "1",
            "serializer": "Protobuf",
            "contract": "application.messages.protobuf.someUser",
            "payload": {
              "Name": "some guy",
              "Id": 1
            },
            "headers": {
              "Timestamp": "2022-06-01T12:00:00.001Z"
            }
         }'

    # Produce a JSON based message serialized with Avro
    $ curl -X 'POST' \
      'http://localhost:5001/topics' \
      -H 'Content-Type: application/json' \
      -d '{
            "topic": "test-topic",
            "key": "1",
            "serializer": "Avro",
            "contract": "application.message.avro.someUser",
            "payload": {
              "Name": "some guy",
              "Id": 1
            },
            "headers": {
              "Timestamp": "2022-06-01T12:00:00.001Z"
            }
         }'

    # Produce a auto generated message serialized with Protobuf
    $ curl -X 'POST' \
      'http://localhost:5001/topics' \
      -H 'Content-Type: application/json' \
      -H 'Auto-Generate-Payload: true' \
      -d '{
            "topic": "test-topic",
            "key": "1",
            "serializer": "Protobuf",
            "contract": "application.message.protobuf.someUser",
            "headers": {
              "Timestamp": "2022-06-01T12:00:00.001Z"
            }
         }'

    # Produce multiple auto generated messages serialized with Protobuf
    $ curl -X 'POST' \
      'http://localhost:5001/topicsBulk' \
      -H 'Content-Type: application/json' \
      -d '{
            "topic": "test-topic",
            "serializer": "Protobuf",
            "contract": "application.message.protobuf.someUser",
            "headers": {
                "Timestamp": "2022-06-01T12:00:00.001Z"
            },
            "numberMessages": 100,
            "propertiesModifier": {
                "Name": "A-Real-Name"
            }
         }'
```

## Contributing

1.  Check the issues or open a new one
2.  Fork this repository
3.  Create your feature branch: `git checkout -b my-new-feature`
4.  Commit your changes: `git commit -am 'feat: Add some feature'`
5.  Push to the branch: `git push origin my-new-feature`
6.  Submit a pull request linked to the issue 1.

## Authors

- [marcosandremartins](https://github.com/marcosandremartins)
- [MiguelCosta](https://github.com/MiguelCosta)

## License

- [MIT](LICENSE)
