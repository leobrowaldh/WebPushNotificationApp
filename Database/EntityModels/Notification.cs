using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.EntityModels
{
    public class Notification
    {
        public int Id { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public int SenderId { get; set; }
        public int ReceiverId {get; set; }
        public string Why { get; set; }
        public bool Sent { get; set; }
        public bool Recieved { get; set; }
        public bool Showed { get; set; }
        public Subscription Subscription { get; set; }
    }
}
