using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories;

public class MessageRepository(WebPushAppContext _db) : IMessageRepository
{
    public async Task<string> GetLastMessageAsync()
    {
        return await _db.Messages.OrderByDescending(x => x.When).Select(x => x.Text).FirstOrDefaultAsync();
    }
    public async Task<List<Message>> GetAllMessagesAsync()
    {
        return await _db.Messages.ToListAsync();
    }

    public async Task AddMessageAsync(Message message)
    {
        await _db.Messages.AddAsync(message);
        await _db.SaveChangesAsync();
    }

    public void ReloadEntity(Message message)
    {
        _db.Entry(message).Reload();
    }
}
