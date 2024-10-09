using Database.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories;

public interface IUserRepository
{
    /// <summary>
    /// Saves the subscription to the database
    /// </summary>
    /// <param name="subscriptionString">a JsonString that represent the subscription object being passed to the database</param>
    /// <param name="userId">The id of the user that is going to subscribe.</param>
    /// <returns>The subscriptionId, null if user not found, or if the subscription was not correctly saved to the database.</returns>
    public Task<int> SaveSubscriptionAsync(string subscriptionString, string userId);
    /// <summary>
    /// Retrieves the subscription from database
    /// </summary>
    /// <param name="subscriberId"></param>
    /// <returns>The subscription as a JSON string, or null if subscription not found in database.</returns>
    public Task<List<Subscription>> GetUserSubscriptionsAsync(string subscriberId);

    /// <summary>
    /// Checks if the subscription endpoint corresponds to the particular user.
    /// </summary>
    /// <param name="subscriptionString"></param>
    /// <param name="userId"></param>
    /// <returns>true if this subscription corresponds to this user, false if it doesn't</returns>
    public Task<bool> IsUserSubscriptionAsync(string subscriptionString, string userId);

    /// <summary>
    /// Removes the subscription from the database
    /// </summary>
    /// <param name="subscriptionString"></param>
    /// <returns>true if successfully removed, false if not.</returns>
    public Task<bool> RemoveSubscriptionAsync(string subscriptionString);
}
