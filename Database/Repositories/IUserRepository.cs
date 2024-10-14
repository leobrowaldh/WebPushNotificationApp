using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories;

public interface IUserRepository
{
    public Task<List<UserContact>?> GetUserContactsAsync(string userId);
    public Task<UserContact?> GetContactByIdAsync(string userId, string contactId);
    public Task<AplicationUser?> AddContactAsync(string userId, string contactId);
    public Task<bool> RemoveContactAsync(string userId, string contactId);
}
