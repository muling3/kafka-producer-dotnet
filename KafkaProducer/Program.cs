using System.Text.Json;
using Confluent.Kafka;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// registering endpoints
app.MapPost("/publish", (ClientReq req) =>
{
    // produce event
    var configuration = new Dictionary<string, string>{
        {"bootstrap.servers", "pkc-75m1o.europe-west3.gcp.confluent.cloud:9092"},
        {"security.protocol", "SASL_SSL"},
        {"sasl.mechanisms", "PLAIN"},
        {"sasl.username", "ZAZ5FCZCGVKTQBJS"},
        {"sasl.password", "h9wAOuBmSXhOdAN6kZJQ4bJQGVGPEmdA5W2Qox5nyjGUyxihLkDmknx1GGM45hlU"},
        {"session.timeout.ms", "45000"}
    };

    var TOPIC = "quickstart_schema";

    using var producer = new ProducerBuilder<string, string>(configuration.AsEnumerable()).Build();

    var eventMsg = new
    {
        PensionerName = req.PensionerName,
        Age = req.age,
        Email = req.email,
        Phone = req.phone
    };

    producer.Produce(TOPIC, new Message<string, string> { Key = req.email, Value = JsonSerializer.Serialize(eventMsg) },
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

    return Results.Ok("Published");
});

app.Run();

record ClientReq(string PensionerName, string email, string phone, int age) { }