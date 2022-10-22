using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class Question
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Value { get; set; }
        public string Head { get; set; }
        public DateTime Date { get; set; }

        public virtual List<Answer> Answers { get; set; }
        public virtual List<File> Files { get; set; }

        public string? CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public string? UserId { get; set; }
        public virtual User User { get; set; }


    }
}
