using System.Diagnostics.Metrics;
using System.Net;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using static Confluent.Kafka.ConfigPropertyNames;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace KafkaProducer.Controllers;

[ApiController]
[Route("[controller]")]
public class ProducerController : ControllerBase
{
    private const string TopicName = "pensioners_to_payroll";

    private readonly IProducer<String, String> _producer;
    private readonly ILogger<ProducerController> _logger;
    private readonly string _topic;

    public ProducerController(IProducer<String, String> producer, ILogger<ProducerController> logger, IOptions<AppProducerConfig> config)
    {
        _producer = producer;
        _logger = logger;
        _topic = config.Value.Topic;

        logger.LogInformation("ProducerController is Active.");
    }

    [HttpGet("Hello")]
    [ProducesResponseType(typeof(String), (int)HttpStatusCode.OK)]
    public String Hello()
    {
        _logger.LogInformation("Hello World");
        return "Hello World";
    }

    [HttpPost("publish")]
    // [ProducesResponseType(typeof(String), (int)HttpStatusCode.Accepted)]
    public async Task<AcceptedResult> SendToTopic(ClientReq req)
    {
        _logger.LogInformation("pensioners_to_payroll");

        var eventMsg = new
        {
            Name = req.Name,
            Age = req.Age,
            Email = req.Email,
            Phone = req.Phone,
            Award = req.Award,
        };

        var result = await _producer.ProduceAsync(TopicName, new Message<string, string> { Key = req.Email, Value = JsonSerializer.Serialize(eventMsg) }
        );

        // release the message
        _producer.Flush(TimeSpan.FromSeconds(10));
        return Accepted("", eventMsg);
    }
}