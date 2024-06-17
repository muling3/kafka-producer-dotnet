using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using Confluent.Kafka;
// using kafkaproducer.services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using KafkaProducer.Controllers;

// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
// builder.Services.AddSingleton<IKafkaProducerSvc, KafkaProducerSvc>();

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseHttpsRedirection();

// // registering endpoints
// app.MapPost("/publish", ([FromBody] ClientReq req, IKafkaProducerSvc svc) =>
// {
//     var TOPIC = "quickstart_schema";

//     var eventMsg = new ClientReq(req.PensionerName, req.Email, req.Phone, req.Age);
//     svc.produceMessage(eventMsg, TOPIC);

//     return Results.Ok("Published");
// });

// app.Run();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// 1. ******* USING PRODUCER CONFIG ********
// builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection("Kafka-Local"));

// builder.Services.AddSingleton<IProducer<String, String>>(sp =>
// {
//     var config = sp.GetRequiredService<IOptions<ProducerConfig>>();

//     return new ProducerBuilder<String, String>(config.Value)
//         .Build();
// });


// 2. ****** USING CUSTOM CONFIG ******
builder.Services.Configure<AppProducerConfig>(builder.Configuration.GetSection("Producer"));

builder.Services.Configure<SchemaRegistryConfig>(builder.Configuration.GetSection("Schema"));

builder.Services.AddSingleton<ISchemaRegistryClient>(sp =>
{
    var config = sp.GetRequiredService<IOptions<SchemaRegistryConfig>>();

    return new CachedSchemaRegistryClient(config.Value);
});

builder.Services.AddSingleton<ProducerConfig>(sp =>
{
    var config = sp.GetRequiredService<IOptions<AppProducerConfig>>().Value;

    return new ProducerConfig
    {
        BootstrapServers = config.BootstrapServers,
    };
});

builder.Services.AddSingleton<IProducer<string, ClientReq>>(sp =>
{
    var config = sp.GetRequiredService<ProducerConfig>();
    var schemaRegistry = sp.GetRequiredService<ISchemaRegistryClient>();

    return new ProducerBuilder<string, ClientReq>(config)
        .SetValueSerializer(new JsonSerializer<ClientReq>(schemaRegistry))
        .Build();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
