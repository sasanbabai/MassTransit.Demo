namespace Components.Consumers;

using MassTransit;
using Contracts;

public class SubmitOrderConsumer : IConsumer<SubmitOrder>
{
    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {
        if (context.RequestId == null)
        {
            return;
        }

        if (context.Message.CustomerId == Guid.Empty)
        {
            await context.RespondAsync<OrderSubmissionRejected>(new
            {
                context.Message.OrderId,
                context.Message.CustomerId,
                context.Message.Timestamp,
                Reason = "CustomerId is required",
            });
            return;
        }

        await context.Publish<OrderSubmitted>(new
        {
            context.Message.OrderId,
            context.Message.CustomerId,
            context.Message.Timestamp,
        });

        await context.RespondAsync<OrderSubmissionAccepted>(new
        {
            OrderId = context.Message.OrderId,
            CustomerId = context.Message.CustomerId,
            Tiemstamp = InVar.Timestamp,
        });
    }
}