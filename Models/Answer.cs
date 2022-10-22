using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class Answer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Value { get; set; }
        public DateTime Date { get; set; }

        public virtual List<File> Files { get; set; }
        public virtual List<Rating> Ratings { get; set; }
        public string? UserId { get; set; }
        public virtual User User { get; set; }
        public string? QuestionId { get; set; }
        public virtual Question Question { get; set; }

    }
}
