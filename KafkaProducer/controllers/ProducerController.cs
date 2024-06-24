using System.Diagnostics.Metrics;
using System.Net;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using static Confluent.Kafka.ConfigPropertyNames;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Avro;

namespace KafkaProducer.Controllers;

public record ClientReq(string name, string email, string phone, int age, string award)
{
}

[ApiController]
[Route("[controller]")]
public class ProducerController : ControllerBase
{
    private readonly IProducer<string, Pensioner> _producer;
    private readonly ILogger<ProducerController> _logger;
    private readonly string _topic;

    public ProducerController(IProducer<string, Pensioner> producer, ILogger<ProducerController> logger, IOptions<AppProducerConfig> config)
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
    public async Task<AcceptedResult> SendToTopic(Pensioner req)
    {
        _logger.LogInformation($" TOPIC IS :: {_topic}");

        _logger.LogInformation(" EVENT MSG {}", req.ToString());

        var result = await _producer.ProduceAsync(_topic, new Message<string, Pensioner> { Key = req.email, Value = req });
        _logger.LogInformation(" RESULT KEY {} : VALUE {} DELIVERY STATUS: {}", result.Key, result.Value.ToString(), result.Status.ToString());

        // release the message
        _producer.Flush(TimeSpan.FromSeconds(10));
        return Accepted("", req);
    }
}