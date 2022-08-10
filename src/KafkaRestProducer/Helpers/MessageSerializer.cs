namespace KafkaRestProducer.Helpers;

using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using AutoFixture;
using AutoFixture.Kernel;
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

    private readonly Fixture fixture;

    public MessageSerializer(Settings settings)
    {
        this.settings = settings;
        this.fixture = new Fixture();
    }

    public object Serialize(
        SerializerType serializer,
        string contract,
        object? payload,
        bool autoGeneratePayload)
    {
        var jsonString = JsonSerializer.Serialize(payload, this.jsonOptions);

        if (serializer == SerializerType.Json)
        {
            return JObject.Parse(jsonString);
        }

        var type = this.GetTypeFromAssemblies(contract);

        if (type == null)
        {
            throw new InvalidDataException($"{contract} not found");
        }

        if (autoGeneratePayload)
        {
            return new SpecimenContext(this.fixture).Resolve(type);
        }

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
        var result = JsonSerializer.Deserialize(stream, type);

        if (result == null)
        {
            throw new SerializationException($"{contract} failed serialization");
        }

        return result;
    }

    public List<object> Serialize(
        SerializerType serializer,
        string contract,
        int numberMessages)
    {
        return Enumerable.Range(1, numberMessages)
            .Select(x => Serialize(serializer, contract, null, true))
            .ToList();
    }

    private Type? GetTypeFromAssemblies(string contract)
    {
        var assemblies = new List<Assembly>();

        foreach (var assemblyFile in Directory.GetFiles(this.settings.ContractsFolder, "*.dll"))
        {
            try
            {
                assemblies.Add(Assembly.LoadFrom(assemblyFile));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        return assemblies.Select(assembly => assembly.GetType(contract)).FirstOrDefault(type => type != null);
    }
}
