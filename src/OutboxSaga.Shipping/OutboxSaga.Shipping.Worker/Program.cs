using Confluent.Kafka;
using OutboxSaga.Shipping.Infrastructure;
using OutboxSaga.Shipping.Worker;
using OutboxSaga.Shipping.Worker.Handlers;
using OutboxSaga.Shipping.Worker.Publishers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection("Kafka:Producer"));
builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection("Kafka:Consumer"));

builder.Services.AddSingleton<OrderPaidHandler>();

builder.Services.AddHostedService<KafkaConsumerService>();
builder.Services.AddHostedService<OutboxPublisherService>();

var host = builder.Build();
host.Run();
