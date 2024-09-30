namespace Database.Repositories;

public class UserRepository(WebPushAppContext _db): IUserRepository
{
    /// <summary>
    /// Retrieves the subscribed user from database
    /// </summary>
    /// <param name="subscriberId"></param>
    /// <returns>the user object, or null if not found.</returns>
    public async Task<User?> GetSubscriptionAsync(int subscriberId) => await _db.Users.FindAsync(subscriberId);

    /// <summary>
    /// Saves the subscription to the database
    /// </summary>
    /// <param name="subscription">a JsonString that represent the subscription object being passed to the database</param>
    /// <returns>true if correctly saved to the database, false if it was not saved.</returns>
    public async Task<int> SaveSubscriptionAsync(string subscription)
    {
        User user = new() { SubscriptionJson = subscription};
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
        return user.Id; // userId will be 0 if it was not added to the db
    }
}
