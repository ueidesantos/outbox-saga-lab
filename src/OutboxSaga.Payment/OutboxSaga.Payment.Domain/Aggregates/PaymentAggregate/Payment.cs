using OutboxSaga.Payment.Domain.Common;
using OutboxSaga.Payment.Domain.Events;

namespace OutboxSaga.Payment.Domain.Aggregates.PaymentAggregate;

public class Payment : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public string Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public Payment(Guid orderId, decimal amount)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        Amount = amount;
        Status = "Authorized";
        CreatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new PaymentAuthorizedDomainEvent(Id, OrderId, Amount, CreatedAtUtc));
    }

    // Constructor for rehydration
    private Payment(Guid id, Guid orderId, decimal amount, string status, DateTime createdAtUtc)
    {
        Id = id;
        OrderId = orderId;
        Amount = amount;
        Status = status;
        CreatedAtUtc = createdAtUtc;
    }

    public static Payment Rehydrate(Guid id, Guid orderId, decimal amount, string status, DateTime createdAtUtc)
    {
        return new Payment(id, orderId, amount, status, createdAtUtc);
    }
}
