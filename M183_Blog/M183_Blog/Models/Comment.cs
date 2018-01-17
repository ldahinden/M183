using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace M183_Blog.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public virtual User User { get; set; }
        public virtual Post Post { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
    }
}