namespace KafkaRestProducer.Kafka;

using KafkaRestProducer.Models;

public interface IMessageSerializer
{
    object Serialize(SerializerType serializer, string contract, bool autoGeneratePayload, object? payload);

    public List<object> Serialize(string contract, int numberMessages);
}
