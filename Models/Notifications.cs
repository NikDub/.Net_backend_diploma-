using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class Notifications
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Type { get; set; }
        public bool Seen { get; set; }
        public DateTime Date { get; set; }
        public string? UserSubId { get; set; }

        public string? UserId { get; set; }
        public virtual User User { get; set; }

        public string? QuestionId { get; set; }
        public virtual Question Question { get; set; }
        public string? AnswerId { get; set; }
        public virtual Answer Answer { get; set; }
        public string? RatingId { get; set; }
        public virtual Rating Rating { get; set; }
    }
}
