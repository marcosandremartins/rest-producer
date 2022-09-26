namespace KafkaRestProducer.Kafka;

using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using AutoFixture;
using AutoFixture.Kernel;
using KafkaRestProducer.Models;
using KafkaRestProducer.Wrappers;
using Newtonsoft.Json.Linq;

public class MessageSerializer : IMessageSerializer
{
    private readonly JsonSerializerOptions? jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IAssemblyWrapper assemblyWrapper;

    private readonly Fixture fixture;

    public MessageSerializer(IAssemblyWrapper assemblyWrapper)
    {
        this.assemblyWrapper = assemblyWrapper;
        this.fixture = new Fixture();
    }

    public object Serialize(
        SerializerType serializer,
        string contract,
        bool autoGeneratePayload,
        object? payload)
    {
        var serializedPayload = JsonSerializer.Serialize(payload, this.jsonOptions);

        if (serializer == SerializerType.Json && payload is not null)
        {
            return JObject.Parse(serializedPayload);
        }

        var contractType = this.assemblyWrapper.GetType(contract);

        return autoGeneratePayload
            ? this.DoSerialize(contractType)
            : this.DoSerialize(contractType, serializedPayload);
    }

    public List<object> Serialize(
        string contract,
        int numberOfMessages)
    {
        var contractType = this.assemblyWrapper.GetType(contract);

        return Enumerable.Range(1, numberOfMessages)
            .Select(x => this.DoSerialize(contractType))
            .ToList();
    }

    private object DoSerialize(Type contractType)
        => new SpecimenContext(this.fixture).Resolve(contractType);

    private object DoSerialize(
        Type contractType,
        string payload)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(payload));

        var result = JsonSerializer.Deserialize(stream, contractType);

        if (result == null)
        {
            throw new SerializationException($"Contract '{nameof(contractType)}' failed serialization.");
        }

        return result;
    }
}
