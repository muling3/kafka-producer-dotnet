using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using Confluent.Kafka;
using kafkaproducer.services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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

builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection("Kafka-Local"));

builder.Services.AddSingleton<IProducer<String, String>>(sp =>
{
    var config = sp.GetRequiredService<IOptions<ProducerConfig>>();

    return new ProducerBuilder<String, String>(config.Value)
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


public record ClientReq(string PensionerName, string Email, string Phone, int Age)
{
}