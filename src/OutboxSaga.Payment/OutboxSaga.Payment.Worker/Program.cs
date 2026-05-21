using Confluent.Kafka;
using OutboxSaga.Payment.Infrastructure;
using OutboxSaga.Payment.Worker;
using OutboxSaga.Payment.Worker.Handlers;
using OutboxSaga.Payment.Worker.Publishers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection("Kafka:Producer"));
builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection("Kafka:Consumer"));

builder.Services.AddSingleton<OrderCreatedHandler>();

builder.Services.AddHostedService<KafkaConsumerService>();
builder.Services.AddHostedService<OutboxPublisherService>();

var host = builder.Build();
host.Run();
