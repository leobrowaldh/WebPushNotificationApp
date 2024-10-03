using Microsoft.Extensions.Logging;

namespace Database.Repositories;

public class UserRepository(WebPushAppContext _db, ILogger<UserRepository> _logger): IUserRepository
{
    public async Task<string?> GetUserSubscriptionsAsync(string subscriberId)
    {
        Subscription? subscription = await _db.Subscriptions.FirstOrDefaultAsync(s => s.UserId == subscriberId);
        return subscription?.SubscriptionJson;
    }

    
    public async Task<int> SaveSubscriptionAsync(string subscriptionString, string userId)
    {
        AplicationUser? user = await _db.Users.FindAsync(userId);
        if (user == null) 
        {
            _logger.LogError("user with id = {userId} was not found in database", userId);
            return 0; 
        }
        Subscription subscription = new() { SubscriptionJson = subscriptionString };
        user.Subscriptions.Add(subscription);
        int success = await _db.SaveChangesAsync();
        if (success == 0) 
        {
            _logger.LogError("Failed to save subscription to database.");
            return 0; 
        }
        return subscription.Id;
    }
}
