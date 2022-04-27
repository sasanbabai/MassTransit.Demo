using MassTransit;

namespace Components.StateMachines;

public class OrderSagaDefinition : SagaDefinition<OrderSagaInstance>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderSagaInstance> sagaConfigurator)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(5)));
        endpointConfigurator.UseInMemoryOutbox();
    }
}