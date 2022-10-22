using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class AppUserModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name  { get; set; }   
        public byte[] Image  { get; set; }

        public int QuestionCount { get; set; }
        public int RatingCount { get; set; }

    }
}
