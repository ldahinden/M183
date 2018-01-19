using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace M183_Blog.Models
{
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public virtual User User { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Deleted { get; set; }
        public DateTime Timestamp { get; set; }
    }
}