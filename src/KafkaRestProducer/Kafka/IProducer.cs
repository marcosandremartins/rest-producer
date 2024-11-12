namespace KafkaRestProducer.Kafka;

using Confluent.Kafka;
using KafkaRestProducer.Configuration;
using KafkaRestProducer.Models;

public interface IProducer
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
        bool addGzipMessageCompressor);

    public Task Produce(
        IEnumerable<string> brokers,
        string schemaRegistryUrl,
        SchemaRegistryAuth schemaRegistryAuth,
        string topic,
        SerializerType serializer,
        List<object> messages,
        CompressionType compressionType,
        Dictionary<string, string> messageHeaders,
        bool addGzipMessageCompressor);
}
