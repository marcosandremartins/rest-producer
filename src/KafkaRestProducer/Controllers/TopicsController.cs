namespace KafkaRestProducer.Controllers;

using KafkaRestProducer.Configuration;
using KafkaRestProducer.Kafka;
using KafkaRestProducer.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
public class TopicsController : ControllerBase
{
    private readonly IMessageSerializer messageSerializer;
    private readonly IProducer producer;
    private readonly Settings settings;

    public TopicsController(
        IMessageSerializer messageSerializer,
        IProducer producer,
        Settings settings)
    {
        this.messageSerializer = messageSerializer;
        this.producer = producer;
        this.settings = settings;
    }

    [HttpPost("topics")]
    [ProducesResponseType(statusCode: 202)]
    [ProducesResponseType(statusCode: 400, Type = typeof(ProblemDetails))]
    [ProducesResponseType(statusCode: 500, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> PostAsync(
        [FromBody] MessageRequest messageRequest,
        [FromHeader(Name = "Auto-Generate-Payload")]
        bool autoGeneratePayload = false)
    {
        messageRequest.Validate(autoGeneratePayload);

        var message = this.messageSerializer.Serialize(
            messageRequest,
            autoGeneratePayload);

        await this.producer.Produce(
            this.settings.KafkaBrokers,
            this.settings.SchemaRegistryUrl,
            messageRequest.Topic,
            messageRequest.Serializer,
            messageRequest.Key,
            message,
            messageRequest.Headers
        );

        return Accepted();
    }

    [HttpPost("topicsBulk")]
    [ProducesResponseType(statusCode: 202)]
    [ProducesResponseType(statusCode: 400, Type = typeof(ProblemDetails))]
    [ProducesResponseType(statusCode: 500, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> PostBulkAsync([FromBody] MessageRequestBulk messageRequest)
    {
        messageRequest.Validate();

        var messages = this.messageSerializer.BulkSerialize(messageRequest);

        await this.producer.Produce(
            this.settings.KafkaBrokers,
            this.settings.SchemaRegistryUrl,
            messageRequest.Topic,
            messageRequest.Serializer,
            messages,
            messageRequest.Headers
        );

        return Accepted();
    }
}
