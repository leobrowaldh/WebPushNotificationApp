using Database.EntityModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Database;

//ApplicationUser will replace the normal inherited user entity from identity
public class WebPushAppContext : IdentityDbContext<AplicationUser>
{
    public WebPushAppContext(DbContextOptions<WebPushAppContext> options)
            : base(options)
    {
    }

    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<UserContact> UserContacts {  get; set; }

    // Handling the self referencing User relationship:
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the self-referencing many-to-many relationship using the explicit UserContacts entity
        modelBuilder.Entity<UserContact>()
            .HasKey(uc => new { uc.UserId, uc.ContactId });  // Composite key

        modelBuilder.Entity<UserContact>()
            .HasOne(uc => uc.User)
            .WithMany(u => u.Contacts)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Restrict);  // No cascading delete on user removal

        modelBuilder.Entity<UserContact>()  // Korrigera här
            .HasOne(uc => uc.Contact)
            .WithMany()  // Contact side doesn't need a collection
            .HasForeignKey(uc => uc.ContactId)
            .OnDelete(DeleteBehavior.Restrict);  // No cascading delete on contact removal

        // Configure the relationship between UserContacts and Messages
        modelBuilder.Entity<Message>()
            .HasOne(m => m.UserContact)  // Korrigera här
            .WithMany(uc => uc.Messages)
            .HasForeignKey(m => new { m.UserId, m.ContactId })  // Korrigera här
            .OnDelete(DeleteBehavior.Cascade);  // Deleting UserContacts deletes messages

        // Existing Subscription configuration remains the same
        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.User)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(s => s.UserId);
    }
}
