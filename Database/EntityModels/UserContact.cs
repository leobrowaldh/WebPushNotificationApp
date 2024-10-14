using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.EntityModels
{
    public class UserContact
    {
        public string UserId { get; set; }
        public AplicationUser User { get; set; }

        public string ContactId { get; set; }
        public AplicationUser Contact { get; set; }

        // Collection of messages in the chat between User and Contact
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
