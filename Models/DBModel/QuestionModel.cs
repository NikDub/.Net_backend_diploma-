using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models.DBModel
{
    public class QuestionModel
    {
        [FromForm(Name = "head")]
        public string Head { get; set; }
        [FromForm(Name = "value")]
        public string Value { get; set; }
        [FromForm(Name = "date")]
        public DateTime Date { get; set; }
        [FromForm(Name = "category")]
        public string Category { get; set; }
    }
}
