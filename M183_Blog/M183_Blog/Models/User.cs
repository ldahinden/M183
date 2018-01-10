using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace M183_Blog.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
        public bool Active { get; set; }
    }
}