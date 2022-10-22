using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class User : IdentityUser
    {
        [Column(TypeName ="nvarchar(150)")]
        public string Name { get; set; }
        public DateTime Date { get; set; }

        public virtual List<Subscription> subscriptions { get; set; }
        public virtual List<Question> Questions { get; set; }
        public virtual List<Rating> Ratings { get; set; }
        public virtual List<File> Files { get; set; }
        public virtual List<Answer> Answers { get; set; }
    }
}
