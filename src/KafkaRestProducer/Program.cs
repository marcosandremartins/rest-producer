using System.Text.Json.Serialization;
using Hellang.Middleware.ProblemDetails;
using KafkaRestProducer.Configuration;
using KafkaRestProducer.Converters;
using KafkaRestProducer.Kafka;
using KafkaRestProducer.Wrappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails(ProblemDetailsOptionsExtensions.Options());
builder.Services.AddSwaggerGen();

var settings = builder.Configuration.GetSection("Settings").Get<Settings>();
builder.Services.AddSingleton(settings);
builder.Services.AddSingleton<IProducer, Producer>();
builder.Services.AddSingleton<IMessageSerializer, MessageSerializer>();
builder.Services.AddSingleton<IAssemblyWrapper, AssemblyWrapper>();

var app = builder.Build();

// Configure NewtonsoftJson globally to use DecimalToAvroDecimalConverter
JsonConvert.DefaultSettings = () => new JsonSerializerSettings
{
    Converters = { new DecimalToAvroDecimalConverter() }
};

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();
app.UseProblemDetails();
app.MapControllers();

app.Run();
