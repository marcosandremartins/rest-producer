namespace KafkaRestProducer.Controllers;

using System.Runtime.Serialization;
using KafkaRestProducer.Configuration;
using Microsoft.AspNetCore.Mvc;
using KafkaRestProducer.Models;
using KafkaRestProducer.Helpers;

[ApiController]
[Route("[controller]")]
public class TopicsController : ControllerBase
{
    private readonly MessageSerializer messageSerializer;
    private readonly Settings settings;

    public TopicsController(
        MessageSerializer messageSerializer,
        Settings settings)
    {
        this.messageSerializer = messageSerializer;
        this.settings = settings;
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] MessageRequest messageRequest)
    {
        var message = this.messageSerializer.Serialize(
            messageRequest.Serializer,
            messageRequest.Payload,
            messageRequest.Contract);

        if (message == null)
        {
            throw new SerializationException();
        }

        await Producer.Produce(
            this.settings.KafkaBrokers,
            this.settings.SchemaRegistryUrl,
            messageRequest.Topic,
            messageRequest.Serializer,
            messageRequest.Key,
            message,
            messageRequest.Headers
        );

        return Ok();
    }
}
