using System.Diagnostics.Metrics;
using System.Net;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using static Confluent.Kafka.ConfigPropertyNames;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace KafkaProducer.Controllers;

public record ClientReq(string name, string email, string phone, int age, string award)
{
}

[ApiController]
[Route("[controller]")]
public class ProducerController : ControllerBase
{
    private const string TopicName = "test_topic";

    private readonly IProducer<string, ClientReq> _producer;
    private readonly ILogger<ProducerController> _logger;
    private readonly string _topic;

    public ProducerController(IProducer<string, ClientReq> producer, ILogger<ProducerController> logger, IOptions<AppProducerConfig> config)
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
        _logger.LogInformation($" TOPIC IS :: {_topic}");

        var eventMsg = new ClientReq(req.name, req.email, req.phone, req.age, req.award);
        // var eventMsg = new ClientReq(req.name);

        var result = await _producer.ProduceAsync(_topic, new Message<string, ClientReq> { Key = req.email, Value = eventMsg });
        _logger.LogInformation(" RESULT KEY {} : VALUE {} DELIVERY STATUS: {}", result.Key, result.Value.ToString(), result.Status.ToString());

        // release the message
        _producer.Flush(TimeSpan.FromSeconds(10));
        return Accepted("", eventMsg);
    }
}