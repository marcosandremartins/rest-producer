# Kafka REST Producer

Publish any message to Kafka through a RESTFul API

## Requirements

- [Kafka](https://hub.docker.com/r/confluentinc/cp-kafka)
- [Schema Registry](https://hub.docker.com/r/confluentinc/cp-schema-registry)

## Quickstart

Running off a Composer

```yaml
version: '3.4'
services:
  rest-producer:
    image: marcosandremartins/rest-producer:latest
    environment:
      KAFKA_BROKERS: kafka:9092
      SCHEMA_REGISTRY_URL: schemaregistry:8085
    volumes:
      - ./contracts:/app/contracts
    ports:
      - "5001:5001"
```

The following assumes you have Kafka, SchemaRegistry and an instance of the REST Producer running with the appropriate contracts assemblies loaded

```bash
    # Produce a JSON based message
    $ curl -X 'POST' \
      'http://localhost:5001/Topics' \
      -H 'Content-Type: application/json' \
      -d '{
            "topic": "test-topic",
            "key": "1",
            "serializer": "Json",
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
      'http://localhost:5001/Topics' \
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
      'http://localhost:5001/Topics' \
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