namespace KafkaRestProducer.IntegrationTests.ServiceMocks;

using Confluent.Kafka;
using KafkaRestProducer.Configuration;
using KafkaRestProducer.Kafka;
using KafkaRestProducer.Models;

public class FakeProducer : IProducer
{
    public Task Produce(
        IEnumerable<string> brokers,
        string schemaRegistryUrl,
        SchemaRegistryAuth schemaRegistryAuth,
        string topic,
        SerializerType serializer,
        string messageKey,
        object message,
        CompressionType compressionType,
        Dictionary<string, string> messageHeaders,
        bool addGzipMessageCompressor)
        => Task.CompletedTask;

    public Task Produce(
        IEnumerable<string> brokers,
        string schemaRegistryUrl,
        SchemaRegistryAuth schemaRegistryAuth,
        string topic, SerializerType serializer,
        List<object> messages,
        CompressionType compressionType,
        Dictionary<string, string> messageHeaders,
        bool addGzipMessageCompressor)
        => Task.CompletedTask;
}
