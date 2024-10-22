using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories;

public interface IMessageRepository
{
    Task<string> GetLastMessageAsync();
    Task<List<Message>> GetAllMessagesAsync();
    Task AddMessageAsync(Message message);
    void ReloadEntity(Message message);
}
