namespace KafkaRestProducer.Wrappers;

using System.Reflection;
using KafkaRestProducer.Configuration;

public class AssemblyWrapper : IAssemblyWrapper
{
    private readonly Settings settings;
    
    public AssemblyWrapper(Settings settings)
    {
        this.settings = settings;
    }
    
    public Type GetType(string contract)
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

        var type = assemblies.Select(assembly => assembly.GetType(contract)).FirstOrDefault(type => type != null);

        if (type == null)
        {
            throw new DllNotFoundException($"Contract '{contract}' not found.");
        }

        return type;
    }
}
