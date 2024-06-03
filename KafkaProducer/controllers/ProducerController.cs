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
    private const string TopicName = "sample_topic";

    private readonly IProducer<String, String> _producer;
    private readonly ILogger<ProducerController> _logger;

    public ProducerController(IProducer<String, String> producer, ILogger<ProducerController> logger)
    {
        _producer = producer;
        _logger = logger;
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
        _logger.LogInformation("sample_topic");

        var eventMsg = new
        {
            PensionerName = req.PensionerName,
            Age = req.Age,
            Email = req.Email,
            Phone = req.Phone
        };

        var result = await _producer.ProduceAsync(TopicName, new Message<string, string> { Key = req.Email, Value = JsonSerializer.Serialize(eventMsg) }
        //    (deliveryRieport) =>
        //                     {
        //                         Console.WriteLine("delivery REPORT " + JsonSerializer.Serialize(deliveryReport));

        //                         if (deliveryReport.Error.Code != ErrorCode.NoError)
        //                         {
        //                             Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
        //                         }
        //                         else
        //                         {
        //                             Console.WriteLine($"Message published");
        //                         }
        //                     }
        );

        // release the message
        _producer.Flush(TimeSpan.FromSeconds(10));
        return Accepted("", eventMsg);
    }
}