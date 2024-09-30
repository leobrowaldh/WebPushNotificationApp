namespace Database;

public class WebPushAppContext(DbContextOptions<WebPushAppContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}
