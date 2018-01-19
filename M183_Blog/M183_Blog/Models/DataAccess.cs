using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace M183_Blog.Models
{
    public class DataAccess : DbContext
    {
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Token> Token { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserLogin> UserLogin { get; set; }
        public DbSet<Userlog> Userlog { get; set; }
    }
}