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

        if (Serializer != SerializerType.Json && string.IsNullOrWhiteSpace(Contract))
        {
            ValidationMessages.Add($"'{nameof(Contract)}' is required for selected serializer.");
        }

        if (Serializer == SerializerType.Json && autoGeneratePayload)
        {
            ValidationMessages.Add("Action not allowed for selected serializer.");
        }

        if (Serializer == SerializerType.Json && !autoGeneratePayload && Payload is null)
        {
            ValidationMessages.Add($"'{nameof(Payload)}' is required for selected serializer.");
        }

        if (Serializer != SerializerType.Json && !autoGeneratePayload && Payload is null)
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
