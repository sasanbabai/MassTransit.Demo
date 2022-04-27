using Components.Consumers;
using Components.StateMachines;
using MassTransit;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    services.AddMassTransit(cfg =>
    {
        cfg.SetKebabCaseEndpointNameFormatter();
        cfg.AddConsumersFromNamespaceContaining<SubmitOrderConsumer>();
        cfg.UsingRabbitMq((ctx, cfg) =>
        {
            cfg.ConfigureEndpoints(ctx);
        });
        cfg.AddSagaStateMachine<OrderStateMachine, OrderSagaInstance, OrderSagaDefinition>()
            .RedisRepository();
    });

    services.AddHostedService<MassTransitHostedService>();
});

await builder.Build().RunAsync();
