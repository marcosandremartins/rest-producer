namespace KafkaRestProducer.Helpers;

using System.Reflection;
using KafkaRestProducer.Configuration;

public class ContractsManager
{
    private readonly Settings settings;

    public ContractsManager(Settings settings)
    {
        this.settings = settings;
    }

    public Type? GetTypeFromAssemblies(string contract)
    {
        var assemblies = Directory
            .GetFiles(this.settings.ContractsFolder, "*.dll")
            .Select(Assembly.LoadFile)
            .ToList();

        return assemblies.Select(assembly => assembly.GetType(contract)).FirstOrDefault(type => type != null);
    }
}
