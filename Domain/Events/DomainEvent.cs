public abstract class DomainEvent
{
    public DateTime OccurredOn;

    public DomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
    }
}