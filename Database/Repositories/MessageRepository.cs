using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories;

public class MessageRepository(WebPushAppContext _db, ILogger<SubscriptionRepository> _logger) : IMessageRepository
{
    public Task<Message> AddMessageAsync(Message message)
    {
        throw new NotImplementedException();
    }

    public Task<Message> DeleteMessageAsync(int messageId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Message>?> GetContactMessagesAsync(string user, string contact)
    {
        throw new NotImplementedException();
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
