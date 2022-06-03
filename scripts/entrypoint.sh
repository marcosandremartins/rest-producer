#!/bin/bash
sed -i 's|localhost:9092|'"$KAFKA_BROKER"'|g' appsettings.json
dotnet KafkaRestProducer.dll
