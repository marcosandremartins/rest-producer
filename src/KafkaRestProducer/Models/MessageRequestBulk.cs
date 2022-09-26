namespace KafkaRestProducer.Models;

public class MessageRequestBulk
{
    public string Topic { get; set; } = string.Empty;

    public SerializerType Serializer { get; set; }

    public string Contract { get; set; } = string.Empty;

    public Dictionary<string, string> Headers { get; set; } = new();

    public int NumberOfMessages { get; set; } = 0;

    private List<string> ValidationMessages { get; set; } = new();

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Topic))
        {
            ValidationMessages.Add($"Property '{nameof(Topic)}' is Mandatory.");
        }

        if (string.IsNullOrWhiteSpace(Contract))
        {
            ValidationMessages.Add($"Property '{nameof(Contract)}' is Mandatory.");
        }

        if (NumberOfMessages < 1)
        {
            ValidationMessages.Add($"'{nameof(NumberOfMessages)}' must be higher than 0.");
        }

        if (ValidationMessages.Count > 0)
        {
            throw new ArgumentException(string.Join(Environment.NewLine, ValidationMessages));
        }
    }
}
