using MassTransit;

namespace Components.StateMachines;

// The saga instance to preserve the state of the saga
// as well as other useful information
public class OrderSagaInstance : SagaStateMachineInstance, ISagaVersion
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public int Version { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime? Updated { get; set; }
}