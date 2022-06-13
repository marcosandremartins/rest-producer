namespace KafkaRestProducer.Configuration;

public sealed class Settings
{
    public string[] KafkaBrokers { get; set; }

    public string SchemaRegistryUrl { get; set; }

    public string ContractsFolder { get; set; }
}
