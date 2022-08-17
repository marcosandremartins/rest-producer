namespace KafkaRestProducer.Kafka;

using KafkaRestProducer.Models;

public interface IProducer
{
    public Task Produce(
        IEnumerable<string> brokers,
        string schemaRegistryUrl,
        string topic,
        SerializerType serializer,
        string messageKey,
        object message,
        Dictionary<string, string> messageHeaders);

    public Task Produce(
        IEnumerable<string> brokers,
        string schemaRegistryUrl,
        string topic,
        SerializerType serializer,
        List<object> messages,
        Dictionary<string, string> messageHeaders);
}
