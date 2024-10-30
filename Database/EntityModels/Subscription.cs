using System.ComponentModel.DataAnnotations;

namespace Database.EntityModels;

public class Subscription
{
    public int Id { get; private set; }
    public string SubscriptionJson { get; set; }
    public bool IsDeleted { get; set; }
    [Required]
    public string UserId { get; set; }
    public AplicationUser User { get; set; }
}
