namespace KafkaRestProducer;

using System.Text;
using KafkaFlow;
using KafkaFlow.Producers;
using KafkaFlow.Serializer;
using KafkaRestProducer.Models;

public static class Producer
{
    public static async Task Produce(
        IEnumerable<string> brokers,
        string topic,
        SerializerType serializer,
        string messageKey,
        object message,
        Dictionary<string, string> messageHeaders)
    {
        var services = new ServiceCollection();

        services.AddKafka(
            kafka => kafka
                .UseConsoleLog()
                .AddCluster(
                    cluster => cluster
                        .WithBrokers(brokers)
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

        await producers[nameof(KafkaRestProducer)].ProduceAsync(
            messageKey,
            message,
            headers);

        await bus.StopAsync();
    }
}
