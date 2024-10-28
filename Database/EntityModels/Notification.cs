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
        public string SenderId { get; set; }
        public string ReceiverId {get; set; }
        public string Why { get; set; }
        public bool Sent { get; set; } = false;
        public bool InteractedWith { get; set; } = false;
        public Subscription Subscription { get; set; }
    }
}
