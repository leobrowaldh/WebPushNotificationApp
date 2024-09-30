using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories
{
    public interface IUserRepository
    {
        public Task<bool> SaveSubscription(string subscription);
    }
}
