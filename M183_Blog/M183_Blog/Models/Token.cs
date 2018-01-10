using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace M183_Blog.Models
{
    public class Token
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public string Value { get; set; }
        [Timestamp]
        public DateTime TimeStamp { get; set; }
    }
}