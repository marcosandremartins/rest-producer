namespace KafkaRestProducer.Helpers;

using System.Text;
using System.Text.Json;
using KafkaRestProducer.Models;
using Newtonsoft.Json.Linq;

public class MessageSerializer
{
    private readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly ContractsManager contractsManager;

    public MessageSerializer(ContractsManager contractsManager)
    {
        this.contractsManager = contractsManager;
    }

    public object? Serialize(
        SerializerType serializer,
        object payload,
        string contract)
    {
        return serializer switch
        {
            SerializerType.Json => this.JsonSerialize(payload),
            SerializerType.Protobuf => this.ProtobufSerialize(payload, contract),
            _ => throw new ArgumentOutOfRangeException(nameof(serializer), serializer, null)
        };
    }

    private JObject JsonSerialize(object payload)
    {
        var jsonString = JsonSerializer.Serialize(payload, this.jsonOptions);

        return JObject.Parse(jsonString);
    }

    private object? ProtobufSerialize(object payload, string contract)
    {
        var type = this.contractsManager.GetTypeFromAssemblies(contract);

        if (type == null)
        {
            return null;
        }

        var jsonString = JsonSerializer.Serialize(payload, this.jsonOptions);

        var result = JsonSerializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(jsonString)), type);

        return result;
    }
}
