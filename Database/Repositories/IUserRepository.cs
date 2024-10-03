using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Saves the subscription to the database
        /// </summary>
        /// <param name="subscriptionString">a JsonString that represent the subscription object being passed to the database</param>
        /// <param name="userId">The id of the user that is going to subscribe.</param>
        /// <returns>The subscriptionId, 0 if user not found, or if the subscription was not correctly saved to the database.</returns>
        public Task<int> SaveSubscriptionAsync(string subscriptionString, int userId);
        /// <summary>
        /// Retrieves the subscription from database
        /// </summary>
        /// <param name="subscriberId"></param>
        /// <returns>The subscription as a JSON string, or null if subscription not found in database.</returns>
        public Task<string?> GetUserSubscriptionsAsync(int subscriberId);
    }
}
