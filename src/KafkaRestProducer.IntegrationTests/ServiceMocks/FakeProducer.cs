namespace KafkaRestProducer.IntegrationTests.ServiceMocks;

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
        Dictionary<string, string> messageHeaders)
        => Task.CompletedTask;

    public Task Produce(
        IEnumerable<string> brokers,
        string schemaRegistryUrl,
        SchemaRegistryAuth schemaRegistryAuth,
        string topic, SerializerType serializer,
        List<object> messages,
        Dictionary<string, string> messageHeaders)
        => Task.CompletedTask;
}
