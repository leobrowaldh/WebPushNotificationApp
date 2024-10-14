using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories;

public class UserRepository(WebPushAppContext _db, ILogger<SubscriptionRepository> _logger) : IUserRepository
{
    public async Task<AplicationUser?> AddContactAsync(string userId, string contactId)
    {
        AplicationUser? user = await _db.Users.FindAsync(userId);
        if (user == null) 
        {
            _logger.LogError("User not found");
            return null;
        }
        AplicationUser? contact = await _db.Users.FindAsync(userId);
        if (contact == null)
        {
            _logger.LogError("Contact not found");
            return null;
        }

        UserContact userContact = new UserContact
        {
            UserId = userId,
            ContactId = contactId,
            User = user,
            Contact = contact
        };
        user.Contacts.Add(userContact);
        int success = await _db.SaveChangesAsync();
        return success == 0 ? null : contact;
    }

    public async Task<UserContact?> GetContactByIdAsync(string userId, string contactId)
    {
        var userContact = await _db.UserContacts
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.ContactId == contactId);

        return userContact;
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
