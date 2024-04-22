using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using Confluent.Kafka;
using kafkaproducer.services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IKafkaProducerSvc, KafkaProducerSvc>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// registering endpoints
app.MapPost("/publish", ([FromBody] ClientReq req, IKafkaProducerSvc svc) =>
{
    var TOPIC = "quickstart_schema";

    var eventMsg = new ClientReq(req.PensionerName, req.Email, req.Phone, req.Age);
    svc.produceMessage(eventMsg, TOPIC);

    return Results.Ok("Published");
});

app.Run();

public record ClientReq(string PensionerName, string Email, string Phone, int Age)
{
}