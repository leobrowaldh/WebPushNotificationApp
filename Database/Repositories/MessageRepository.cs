using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories;

public class MessageRepository(WebPushAppContext _db, ILogger<SubscriptionRepository> _logger) : IMessageRepository
{
    public async Task<Message?> AddMessageAsync(Message message)
    {
        _db.Messages.Add(message);
        int success = await _db.SaveChangesAsync();
        return success == 0 ? null : message;
    }

    public async Task<bool> DeleteMessageAsync(int messageId)
    {
        Message? messageToRemove = await _db.Messages.FindAsync(messageId);
        if (messageToRemove != null)
        {
            _db.Messages.Remove(messageToRemove);
        }
        int success = await _db.SaveChangesAsync();
        return success != 0;
    }

    public async Task<List<Message>?> GetContactMessagesAsync(string userId, string contactId)
    {
        return await _db.Messages.Where(m => m.UserContact.UserId == userId && m.UserContact.ContactId == contactId)
            .OrderBy(m  => m.When)
            .ToListAsync();
    }

    public Task<Message?> GetMessageAsync(int messageId)
    {
        throw new NotImplementedException();
    }

    public Task<Message> UpdateMessageAsync(Message message)
    {
        throw new NotImplementedException();
    }
}
