using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories;

public interface IMessageRepository
{
    public Task<List<Message>?> GetContactMessagesAsync(string userId, string contactId);
    public Task<Message?> GetMessageAsync(int messageId);
    public Task<Message?> AddMessageAsync(Message message);
    public Task<Message> UpdateMessageAsync(Message message);
    public Task<bool> DeleteMessageAsync(int messageId);
}
