namespace Contracts;

public record CustomerDeleted
{
    public Guid CustomerId { get; init; }
}