using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class AuthentificationContext: IdentityDbContext
    {
        public AuthentificationContext(DbContextOptions<AuthentificationContext> options) : base(options) { }
       
        public DbSet<User> users { get; set; }
        public DbSet<Subscription> subscriptions { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<File> files { get; set; }
        public DbSet<Question> questions { get; set; }
        public DbSet<Answer> answers { get; set; }
        public DbSet<Rating> ratings { get; set; }

    }
}
