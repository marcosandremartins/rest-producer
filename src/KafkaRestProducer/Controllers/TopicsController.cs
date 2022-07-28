namespace KafkaRestProducer.Controllers;

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
    [ProducesResponseType(202)]
    public async Task<IActionResult> PostAsync(
        [FromBody] MessageRequest messageRequest,
        [FromHeader(Name = "Auto-Generate-Payload")] bool autoGeneratePayload = false)
    {
        if (messageRequest.Serializer != SerializerType.Json && string.IsNullOrWhiteSpace(messageRequest.Contract))
        {
            this.ModelState.AddModelError(nameof(messageRequest.Contract), "Contract is required for selected serializer");
        }

        if (!this.ModelState.IsValid)
        {
            return BadRequest(this.ModelState);
        }

        var message = this.messageSerializer.Serialize(
            messageRequest.Serializer,
            messageRequest.Contract,
            messageRequest.Payload,
            autoGeneratePayload);

        await Producer.Produce(
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
}
