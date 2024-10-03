using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Database;

//ApplicationUser will replace the normal inherited user entity from identity
public class WebPushAppContext : IdentityDbContext<ApplicationUser>
{
    public WebPushAppContext(DbContextOptions<WebPushAppContext> options)
            : base(options)
    {
    }

    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<UserContact> UserContacts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Composite key for UserContact
        modelBuilder.Entity<UserContact>()
            .HasKey(uc => new { uc.UserId, uc.ContactUserId });

        // Relationship between UserContact and ApplicationUser
        modelBuilder.Entity<UserContact>()
            .HasOne(uc => uc.User) // Navigation property for User
            .WithMany(u => u.Contacts) // Navigation property in ApplicationUser
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Optional: Define delete behavior

        modelBuilder.Entity<UserContact>()
            .HasOne(uc => uc.ContactUser) // Navigation property for ContactUser
            .WithMany() // No navigation property in ApplicationUser for ContactUser
            .HasForeignKey(uc => uc.ContactUserId)
            .OnDelete(DeleteBehavior.Cascade); // Optional: Define delete behavior
    }

}
