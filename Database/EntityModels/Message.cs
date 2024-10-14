using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.EntityModels
{
    public class Message
    {
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }
        public DateTime When { get; set; } = DateTime.Now;
        public string UserId { get; set; } // Nytt fält
        public string ContactId { get; set; } // Nytt fält
        public UserContact UserContact { get; set; }
    }
}
