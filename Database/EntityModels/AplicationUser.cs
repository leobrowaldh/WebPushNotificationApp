using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Database.Seeder;

namespace Database.EntityModels;

public class AplicationUser : IdentityUser
{
    public string? ProfilePicture { get; set; } = ProfilePictureSeeder.RandomizeProfilePicture();
    public ICollection<Subscription> Subscriptions { get; set; } = [];
    public ICollection<AplicationUser> Contacts { get; set; } = [];
    public virtual ICollection<Message> Messages { get; set; } = [];

}
