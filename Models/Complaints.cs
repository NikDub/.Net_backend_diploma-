using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class Complaints
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Value { get; set; }
        public bool Seen { get; set; }

        public string? UserId { get; set; }
        public virtual User User { get; set; }
        public string? QuestionId { get; set; }
        public virtual Question Question { get; set; }
        public string? AnswerId { get; set; }
        public virtual Answer Answer { get; set; }
    }
}
