using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories
{
    public interface IUserRepository
    {
        public Task<int> SaveSubscriptionAsync(string subscription);
        public Task<User?> GetSubscriptionAsync(int subscriberId);
    }
}
