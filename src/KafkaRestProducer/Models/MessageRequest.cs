using Confluent.Kafka;

namespace KafkaRestProducer.Models;

using System.Text.Json;
using Newtonsoft.Json.Linq;

public class MessageRequest
{
    public string Topic { get; set; } = string.Empty;

    public string Key { get; set; } = string.Empty;

    public SerializerType Serializer { get; set; }

    public string Contract { get; set; } = string.Empty;

    public object? Payload { get; set; }

    public CompressionType CompressionType { get; set; } = CompressionType.None;

    public Dictionary<string, string> Headers { get; set; } = new();

    private List<string> ValidationMessages { get; set; } = new();

    public void Validate(bool autoGeneratePayload)
    {
        if (string.IsNullOrWhiteSpace(Topic))
        {
            ValidationMessages.Add($"Property '{nameof(Topic)}' is Mandatory.");
        }

        if (string.IsNullOrWhiteSpace(Key))
        {
            ValidationMessages.Add($"Property '{nameof(Key)}' is Mandatory.");
        }

        if (string.IsNullOrWhiteSpace(Contract) && autoGeneratePayload)
        {
            ValidationMessages.Add($"'{nameof(Contract)}' is required when auto generate set to true.");
        }

        if (!autoGeneratePayload && Payload is null)
        {
            ValidationMessages.Add($"Property '{nameof(Payload)}' is required.");
        }

        if (Payload is not null)
        {
            try
            {
                var serializedPayload = JsonSerializer.Serialize(Payload);

                JObject.Parse(serializedPayload);
            }
            catch (Exception ex)
            {
                ValidationMessages.Add($"Property '{nameof(Payload)}' is invalid: {ex.Message}");
            }
        }

        if (ValidationMessages.Count > 0)
        {
            throw new ArgumentException(string.Join(Environment.NewLine, ValidationMessages));
        }
    }
}
