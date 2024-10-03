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

    //Needed to clarify foreign key relationship:
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.User)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(s => s.UserId); // Reference 'UserId', which points to IdentityUser's 'Id'
    }

}
