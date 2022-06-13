namespace KafkaRestProducer.Helpers;

using System.Reflection;
using System.Text;
using System.Text.Json;
using KafkaRestProducer.Configuration;
using KafkaRestProducer.Models;
using Newtonsoft.Json.Linq;

public class MessageSerializer
{
    private readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly Settings settings;

    public MessageSerializer(Settings settings)
    {
        this.settings = settings;
    }

    public object? Serialize(
        SerializerType serializer,
        object payload,
        string contract)
    {
        var jsonString = JsonSerializer.Serialize(payload, this.jsonOptions);

        if (serializer == SerializerType.Json)
        {
            return JObject.Parse(jsonString);
        }

        var type = this.GetTypeFromAssemblies(contract);

        return type == null
            ? null
            : JsonSerializer.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(jsonString)), type);
    }

    private Type? GetTypeFromAssemblies(string contract)
    {
        var assemblies = Directory
            .GetFiles(this.settings.ContractsFolder, "*.dll")
            .Select(Assembly.LoadFile)
            .ToList();

        return assemblies.Select(assembly => assembly.GetType(contract)).FirstOrDefault(type => type != null);
    }
}
