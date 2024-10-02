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
        /// <param name="subscription">a JsonString that represent the subscription object being passed to the database</param>
        /// <returns>true if correctly saved to the database, false if it was not saved.</returns>
        public Task<int> SaveSubscriptionAsync(string subscription);
        /// <summary>
        /// Retrieves the subscribed user from database
        /// </summary>
        /// <param name="subscriberId"></param>
        /// <returns>the user object, or null if not found.</returns>
        public Task<User?> GetSubscriptionAsync(int subscriberId);
    }
}
