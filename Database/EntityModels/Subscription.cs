namespace Database.EntityModels;

public class Subscription
{
    public int Id { get; private set; }
    public string? SubscriptionJson { get; set; }
    public int AplicationUserId { get; set; }
}
