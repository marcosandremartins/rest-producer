namespace KafkaRestProducer.Controllers;

using Microsoft.AspNetCore.Mvc;
using KafkaRestProducer.Models;
using KafkaFlow.Producers;
using System.Linq;
using System.Text;
using KafkaFlow;
using System.Text.Json;

[ApiController]
[Route("[controller]")]
public class TopicsController : ControllerBase
{
    private readonly IProducerAccessor producer;
    private readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };


    public TopicsController(IProducerAccessor producerAccessor)
    {
        this.producer = producerAccessor;
    }


/*
{
  "topic": "batatas",
  "key": "batatas1",
  "serializer": 1,
  "payload": {
    "name": "Filipe",
    "nif": 666888555,
    "address": "Maia"
  },
  "headers": [
    {
      "key": "h1",
      "value": "h1v1"
    }
  ]
}
*/

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] MessageRequest messageRequest)
    {
        var headers = new MessageHeaders();

        foreach (var header in messageRequest.Headers)
        {
            headers.Add(header.Key, Encoding.UTF8.GetBytes(header.Value));
        }

        var jsonString = JsonSerializer.Serialize(messageRequest.Payload, jsonOptions);

        var myUser = JsonSerializer.Deserialize<User>(jsonString, jsonOptions);

        await this.producer
            .GetProducer("PrintConsole")
            .ProduceAsync(messageRequest.Key, myUser, headers);

        return Ok();
    }

    class User
    {
        public string name { get; set; }

        public int nif { get; set; }

        public string address { get; set; }
    }
}
