namespace KafkaRestProducer.Controllers;

using Microsoft.AspNetCore.Mvc;
using KafkaRestProducer.Models;

[ApiController]
[Route("[controller]")]
public class TopicsController : ControllerBase
{
    [HttpPost]
    public void Post([FromBody] MessageRequest messageRequest)
    {
        throw new NotImplementedException();
    }
}
