using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class Rating
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public int Value { get; set; }

        public string? UserId { get; set; }
        public virtual User User { get; set; }
        public string? AnswerId { get; set; }
        public virtual Answer Answer { get; set; }
    }
}
