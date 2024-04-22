using System.Text.Json;
using Confluent.Kafka;

namespace kafkaproducer.services;

public class KafkaProducerSvc : IKafkaProducerSvc
{
    readonly IProducer<string, string> producer;

    public KafkaProducerSvc()
    {
        var configuration = new Dictionary<string, string>{
        {"bootstrap.servers", "broker.alexandermuli.dev"}
        // {"security.protocol", "SASL_SSL"},
        // {"sasl.mechanisms", "PLAIN"},
        // {"sasl.username", "ZAZ5FCZCGVKTQBJS"},
        // {"sasl.password", "h9wAOuBmSXhOdAN6kZJQ4bJQGVGPEmdA5W2Qox5nyjGUyxihLkDmknx1GGM45hlU"},
        // {"session.timeout.ms", "45000"}
    };
        producer = new ProducerBuilder<string, string>(configuration.AsEnumerable()).Build();
    }

    public void produceMessage(ClientReq message, string topic)
    {
        producer.Produce(topic, new Message<string, string> { Key = message.Email, Value = JsonSerializer.Serialize(message) },
        (deliveryReport) =>
                            {
                                Console.WriteLine("delivery REPORT " + JsonSerializer.Serialize(deliveryReport));

                                if (deliveryReport.Error.Code != ErrorCode.NoError)
                                {
                                    Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                                }
                                else
                                {
                                    Console.WriteLine($"Message published");
                                }
                            }
            );

        // release the message
        producer.Flush(TimeSpan.FromSeconds(10));
    }
};