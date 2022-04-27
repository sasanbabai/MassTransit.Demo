namespace Contracts;

using System;

public record OrderSubmissionRejected
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public DateTime Timestamp { get; init; }
    public string Reason { get; init; }
}