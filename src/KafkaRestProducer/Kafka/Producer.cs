namespace KafkaRestProducer.Kafka;

using System.Text;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaFlow;
using KafkaFlow.Producers;
using KafkaFlow.Serializer;
using KafkaRestProducer.Configuration;
using KafkaRestProducer.Models;

public class Producer : IProducer
{
    public async Task Produce(
        IEnumerable<string> brokers,
        string schemaRegistryUrl,
        SchemaRegistryAuth schemaRegistryAuth,
        string topic,
        SerializerType serializer,
        string messageKey,
        object message,
        CompressionType compressionType,
        Dictionary<string, string> messageHeaders)
    {
        await Produce(
            brokers,
            schemaRegistryUrl,
            schemaRegistryAuth,
            topic,
            serializer,
            messageKey,
            new List<object>() {message},
            compressionType,
            messageHeaders);
    }

    public async Task Produce(
        IEnumerable<string> brokers,
        string schemaRegistryUrl,
        SchemaRegistryAuth schemaRegistryAuth,
        string topic,
        SerializerType serializer,
        List<object> messages,
        CompressionType compressionType,
        Dictionary<string, string> messageHeaders)
    {
        await Produce(
            brokers,
            schemaRegistryUrl,
            schemaRegistryAuth,
            topic,
            serializer,
            string.Empty,
            messages,
            compressionType,
            messageHeaders);
    }

    private static async Task Produce(
        IEnumerable<string> brokers,
        string schemaRegistryUrl,
        SchemaRegistryAuth schemaRegistryAuth,
        string topic,
        SerializerType serializer,
        string messageKey,
        List<object> messages,
        CompressionType compressionType,
        Dictionary<string, string> messageHeaders)
    {
        var services = new ServiceCollection();

        services.AddKafka(
            kafka => kafka
                .UseConsoleLog()
                .AddCluster(
                    cluster => cluster
                        .WithBrokers(brokers)
                        .WithSchemaRegistry(config =>
                        {
                            config.Url = schemaRegistryUrl;

                            if (schemaRegistryAuth.IsAuthenticated)
                            {
                                config.BasicAuthUserInfo =
                                    $"{schemaRegistryAuth.Username}:{schemaRegistryAuth.Password}";
                            }
                        })
                        .AddProducer(
                            nameof(KafkaRestProducer),
                            producer => producer
                                .DefaultTopic(topic)
                                .WithCompression(compressionType)
                                .AddMiddlewares(m =>
                                {
                                    switch (serializer)
                                    {
                                        case SerializerType.Json:
                                            m.AddSerializer<NewtonsoftJsonSerializer>();
                                            break;
                                        case SerializerType.Protobuf:
                                            m.AddSerializer<ProtobufNetSerializer>();
                                            break;
                                        case SerializerType.Avro:
                                            m.AddSchemaRegistryAvroSerializer(
                                                new AvroSerializerConfig
                                                {
                                                    AutoRegisterSchemas = true,
                                                    SubjectNameStrategy = SubjectNameStrategy.TopicRecord
                                                });
                                            break;
                                    }
                                })
                                .WithAcks(KafkaFlow.Acks.All)
                        )
                )
        );

        var provider = services.BuildServiceProvider();

        var bus = provider.CreateKafkaBus();

        await bus.StartAsync();

        var producers = provider.GetRequiredService<IProducerAccessor>();

        var headers = new MessageHeaders();

        foreach (var header in messageHeaders)
        {
            headers.Add(header.Key, Encoding.UTF8.GetBytes(header.Value));
        }

        foreach (var message in messages)
        {
            await producers[nameof(KafkaRestProducer)].ProduceAsync(
                messageKey,
                message,
                headers,
                null);
        }

        await bus.StopAsync();
    }
}
