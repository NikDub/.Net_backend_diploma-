using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class File
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public byte[] Value { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public DateTime Date { get; set; }

        public string? UserId { get; set; }
        public virtual User User { get; set; }
        public  string? QuestionId { get; set; }
        public virtual Question Question { get; set; }
        public string? AnswerId { get; set; }
        public virtual Answer Answer { get; set; }
    }
}
