using System.ComponentModel.DataAnnotations;

namespace KafkaRestProducer.Models;

public class MessageRequestBulk
{
    [Required]
    public string Topic { get; set; } = string.Empty;

    [Required]
    public SerializerType Serializer { get; set; }

    public string Contract { get; set; } = string.Empty;

    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

    public int NumberMessages { get; set; }
}
