using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Database.EntityModels;

public class AplicationUser : IdentityUser
{
    public string? ProfilePicture { get; set; }
    public ICollection<Subscription> Subscriptions { get; set; } = [];
    public ICollection<UserContact> Contacts { get; set; } = [];

}
