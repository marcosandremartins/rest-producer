namespace KafkaRestProducer.Kafka;

using System.Runtime.Serialization;
using System.Text.Json;
using AutoFixture;
using AutoFixture.Kernel;
using KafkaRestProducer.Models;
using KafkaRestProducer.Wrappers;
using Newtonsoft.Json.Linq;

public class MessageSerializer : IMessageSerializer
{
    private readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IAssemblyWrapper assemblyWrapper;

    private readonly Fixture fixture;

    public MessageSerializer(IAssemblyWrapper assemblyWrapper)
    {
        this.assemblyWrapper = assemblyWrapper;
        this.fixture = new Fixture {RepeatCount = 1};
    }

    public object Serialize(
        MessageRequest message,
        bool autoGeneratePayload)
    {
        var serializedPayload = JsonSerializer.Serialize(message.Payload, this.jsonOptions);

        if (message.Serializer == SerializerType.Json && message.Payload is not null)
        {
            return JObject.Parse(serializedPayload);
        }

        var contractType = this.assemblyWrapper.GetType(message.Contract);

        return autoGeneratePayload
            ? ToAnonymousSerialize(contractType)
            : ToTypeSerialize(contractType, serializedPayload);
    }

    public List<object> BulkSerialize(MessageRequestBulk message)
    {
        var contractType = this.assemblyWrapper.GetType(message.Contract);

        return Enumerable.Range(1, message.NumberOfMessages)
            .Select(_ => this.ToAnonymousSerialize(contractType, message.PropertiesModifier))
            .ToList();
    }

    private static object ToTypeSerialize(
        Type contractType,
        string payload)
    {
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject(payload, contractType);

        if (result == null)
        {
            throw new SerializationException($"Contract '{nameof(contractType)}' failed serialization.");
        }

        return result;
    }

    private object ToAnonymousSerialize(
        Type contractType,
        Dictionary<string, object>? modifiers = null)
    {
        var contract = new SpecimenContext(this.fixture).Resolve(contractType);

        return modifiers == null ? contract : ApplyModifiers(contract, modifiers, contractType);
    }

    private static object ApplyModifiers(
        object instance,
        Dictionary<string, object> modifiers,
        Type contractType)
    {
        var jsonString = JsonSerializer.Serialize(instance);

        var jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString) as JObject;

        foreach (var modifier in modifiers)
        {
            var token = jsonObject?.SelectToken(modifier.Key);

            var jsonValue = new JValue(modifier.Value.ToString());

            if (int.TryParse(modifier.Value.ToString(), out var intValue))
            {
                jsonValue = new JValue(intValue);
            }

            if (bool.TryParse(modifier.Value.ToString(), out var boolValue))
            {
                jsonValue = new JValue(boolValue);
            }

            token?.Replace(new JValue(jsonValue));
        }

        return ToTypeSerialize(contractType, jsonObject!.ToString());
    }
}
