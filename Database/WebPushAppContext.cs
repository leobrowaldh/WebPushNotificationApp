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

    // Handling the self referencing User relationship:
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the self-referencing many-to-many relationship
        modelBuilder.Entity<AplicationUser>()
            .HasMany(u => u.Contacts)
            .WithMany()
            .UsingEntity(j => j.ToTable("UserContacts")); //creating the join table

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.User)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(s => s.UserId);

        modelBuilder.Entity<Message>()
            .HasOne<AplicationUser>(a => a.Sender)
            .WithMany(d => d.Messages)
            .HasForeignKey(d => d.UserId);
    }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Notification> Notifications { get; set; }
}
