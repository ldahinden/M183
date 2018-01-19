using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace M183_Blog.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public virtual User User { get; set; }
        public virtual Post Post { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
    }
}