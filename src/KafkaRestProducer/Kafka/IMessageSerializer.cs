namespace KafkaRestProducer.Kafka;

using KafkaRestProducer.Models;

public interface IMessageSerializer
{
    object Serialize(MessageRequest message, bool autoGeneratePayload);

    public List<object> BulkSerialize(MessageRequestBulk message);
}
