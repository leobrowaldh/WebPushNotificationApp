using Microsoft.AspNetCore.Identity;

namespace Database.EntityModels;

public class ApplicationUser : IdentityUser
{
    public ICollection<Subscription>? Subscriptions { get; set; }
    public ICollection<UserContact>? Contacts { get; set; }
}
