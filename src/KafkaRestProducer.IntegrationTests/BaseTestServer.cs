namespace KafkaRestProducer.IntegrationTests;

using KafkaRestProducer.Configuration;
using KafkaRestProducer.IntegrationTests.ServiceMocks;
using KafkaRestProducer.Kafka;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class BaseTestServer
{
    protected HttpClient TestHttpClient { get; }

    protected BaseTestServer()
    {
        var application = new Application();

        this.TestHttpClient = application.CreateClient();
    }
}

internal class Application : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder
            .ConfigureServices(services =>
            {
                services.AddSingleton<IProducer, FakeProducer>();
                services.AddSingleton(new Settings { ContractsFolder = "./"});
            });

        return base.CreateHost(builder);
    }
}
