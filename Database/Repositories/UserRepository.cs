namespace Database.Repositories;

public class UserRepository(WebPushAppContext _db): IUserRepository
{
    public async Task<User?> GetSubscriptionAsync(int subscriberId) => await _db.Users.FindAsync(subscriberId);

    
    public async Task<int> SaveSubscriptionAsync(string subscription)
    {
        User user = new() { SubscriptionJson = subscription};
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
        return user.Id; // userId will be 0 if it was not added to the db
    }
}
