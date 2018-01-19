using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace M183_Blog.Models
{
    public class UserLogin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public virtual User User { get; set; }
        public string Ip { get; set; }
        public string SessionId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}