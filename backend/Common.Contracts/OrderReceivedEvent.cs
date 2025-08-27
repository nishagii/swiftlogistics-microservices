namespace Common.Contracts;

//'record' is a modern C# best practice for creating immutable data carriers.
public record OrderReceivedEvent
{
    public Guid OrderId { get; init; }
    public string ClientName { get; init; }
    public string DeliveryAddress { get; init; }
    public DateTime Timestamp { get; init; }
}