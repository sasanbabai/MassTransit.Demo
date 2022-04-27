namespace Contracts;

using System;

public record OrderSubmissionAccepted
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public DateTime Timestamp { get; init; }
}