namespace KafkaRestProducer.Configuration;

public sealed class Settings
{
    public string[] KafkaBrokers { get; set; } = Array.Empty<string>();

    public string SchemaRegistryUrl { get; set; } = string.Empty;

    public SchemaRegistryAuth SchemaRegistryAuth { get; set; }

    public string ContractsFolder { get; set; } = string.Empty;
}
