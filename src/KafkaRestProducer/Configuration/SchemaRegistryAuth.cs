namespace KafkaRestProducer.Configuration;

public class SchemaRegistryAuth
{
    public string Username { get; set; }

    public string Password { get; set; }

    public bool IsAuthenticated =>
        !string.IsNullOrWhiteSpace(this.Username) && !string.IsNullOrWhiteSpace(this.Password);
}
