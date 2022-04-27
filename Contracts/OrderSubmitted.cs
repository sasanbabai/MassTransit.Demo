namespace Contracts;

public record OrderSubmitted
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public DateTime Timestamp { get; init; }
}