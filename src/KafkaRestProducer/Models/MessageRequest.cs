namespace KafkaRestProducer.Models;

public class MessageRequest
{
    public string Topic { get; set; }

    public string Key { get; set; }

    public SerializerType Serializer { get; set; }

    public string Contract { get; set; }

    public object Payload { get; set; }

    public Dictionary<string, string> Headers { get; set; }
}
