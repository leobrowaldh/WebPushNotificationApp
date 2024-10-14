using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories;

public class UserRepository(WebPushAppContext _db, ILogger<SubscriptionRepository> _logger) : IUserRepository
{
    public Task<string> AddContactAsync(string userId, string contactId)
    {
        throw new NotImplementedException();
    }

    public Task<UserContact> GetContactByIdAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<UserContact>?> GetUserContactsAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveContactAsync(string userId, string contactId)
    {
        throw new NotImplementedException();
    }
}
