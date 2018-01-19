using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace M183_Blog.Models
{
    public class Token
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public virtual User User { get; set; }
        public string Value { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool Active { get; set; }
    }
}