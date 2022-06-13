using System.ComponentModel.DataAnnotations;

namespace KafkaRestProducer.Models;

public class MessageRequest
{
    [Required]
    public string Topic { get; set; } = string.Empty;

    [Required]
    public string Key { get; set; } = string.Empty;

    [Required]
    public SerializerType Serializer { get; set; }

    public string Contract { get; set; } = string.Empty;

    [Required]
    public object Payload { get; set; } = new object();

    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
}
