using Contracts;
using MassTransit;

namespace Components.StateMachines;

public class OrderStateMachine : MassTransitStateMachine<OrderSagaInstance>
{
    // Consumes
    public Event<OrderSubmitted> OrderSubmitted { get; private set; }
    public Event<CheckOrder> CheckOrder { get; private set; }
    public Event<CustomerDeleted> CustomerDeleted { get; private set; }

    // States
    public State Submitted { get; private set; }
    public State Canceled { get; private set; }

    public OrderStateMachine()
    {
        // Configure the state machine which property
        // of the instance to use to store the current state
        InstanceState(x => x.CurrentState);

        // Correlate the events with the state machine
        // Helps the state machine to know which instance to load
        // when the event is received
        Event(() => OrderSubmitted, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => CustomerDeleted, x => x.CorrelateBy((saga, context) => saga.CustomerId == context.Message.CustomerId));
        Event(() => CheckOrder, x =>
        {
            x.CorrelateById(context => context.Message.OrderId);
            x.OnMissingInstance(
                m => m.ExecuteAsync(async ctx =>
                    await ctx.RespondAsync<OrderNotFound>(new
                    {
                        OrderId = ctx.Message.OrderId,
                    })));
        });

        // During the Initial state when received OrderSubmitted
        // update the instance and transition to Submitted state.
        Initially(
            When(OrderSubmitted)
                .Then(ctx =>
                {
                    ctx.Saga.CustomerId = ctx.Message.CustomerId;
                    ctx.Saga.Updated = DateTime.UtcNow;
                })
                .TransitionTo(Submitted),
            When(CustomerDeleted)
                .TransitionTo(Canceled));

        // During any state but Initial and Final
        // update the instance and don't transition
        DuringAny(
            When(OrderSubmitted)
                .Then(ctx =>
                {
                    //throw new Exception("Intentional exception");
                    ctx.Saga.CustomerId = ctx.Message.CustomerId;
                    ctx.Saga.Updated = DateTime.UtcNow;
                }));

        // During the Submitted state ignore OrderSubmitted
        During(Submitted,
            Ignore(OrderSubmitted));

        DuringAny(
            When(CustomerDeleted)
                .TransitionTo(Canceled));

        DuringAny(
            When(CheckOrder)
                .RespondAsync(context => context.Init<OrderStatus>(new
                {
                    OrderId = context.Data.OrderId,
                    State = context.Saga.CurrentState,
                })));
    }
}
