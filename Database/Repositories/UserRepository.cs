namespace Database.Repositories;

public class UserRepository(WebPushAppContext _db): IUserRepository
{
    /// <summary>
    /// Saves the subscription to the database
    /// </summary>
    /// <param name="subscription">a JsonString that represent the subscription object being passed to the database</param>
    /// <returns>true if correctly saved to the database, false if it was not saved.</returns>
    public async Task<bool> SaveSubscription(string subscription)
    {
        User user = new() { SubscriptionJson = subscription};
        await _db.Users.AddAsync(user);
        int dbChanges = await _db.SaveChangesAsync();
        return dbChanges > 0;
    }
}
