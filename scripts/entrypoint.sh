#!/bin/bash
sed -i 's|localhost:9094|'"$KAFKA_BROKERS"'|g' appsettings.json
sed -i 's|localhost:8085|'"$SCHEMA_REGISTRY_URL"'|g' appsettings.json
dotnet KafkaRestProducer.dll
