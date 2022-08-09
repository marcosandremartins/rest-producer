namespace KafkaRestProducer;

using System.Text;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaFlow;
using KafkaFlow.Producers;
using KafkaFlow.Serializer;
using KafkaRestProducer.Models;

public static class Producer
{
    public static async Task Produce(
        IEnumerable<string> brokers,
        string schemaRegistryUrl,
        string topic,
        SerializerType serializer,
        string messageKey,
        object message,
        Dictionary<string, string> messageHeaders)
    {
        await Produce(
            brokers,
            schemaRegistryUrl,
            topic,
            serializer,
            messageKey,
            new List<object>() { message },
            messageHeaders);
    }

    public static async Task Produce(
        IEnumerable<string> brokers,
        string schemaRegistryUrl,
        string topic,
        SerializerType serializer,
        List<object> messages,
        Dictionary<string, string> messageHeaders)
    {
        await Produce(
            brokers,
            schemaRegistryUrl,
            topic,
            serializer,
            string.Empty,
            messages,
            messageHeaders);
    }

    private static async Task Produce(
        IEnumerable<string> brokers,
        string schemaRegistryUrl,
        string topic,
        SerializerType serializer,
        string messageKey,
        List<object> messages,
        Dictionary<string, string> messageHeaders)
    {
        var services = new ServiceCollection();

        services.AddKafka(
            kafka => kafka
                .UseConsoleLog()
                .AddCluster(
                    cluster => cluster
                        .WithBrokers(brokers)
                        .WithSchemaRegistry(config => config.Url = schemaRegistryUrl)
                        .AddProducer(
                            nameof(KafkaRestProducer),
                            producer => producer
                                .DefaultTopic(topic)
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
                                .WithAcks(Acks.All)
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
                headers);
        }

        await bus.StopAsync();
    }
}
