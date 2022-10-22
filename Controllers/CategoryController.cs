using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nancy.Json;
using WebApplication2.Models;
using WebApplication2.Models.DBModel;


namespace WebApplication2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private UserManager<User> _userManager;
        private AuthentificationContext _db;
        public CategoryController(UserManager<User> userManager, AuthentificationContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpGet]
        [Route("GetCategory")]
        public List<Category> GetCategory()
        {
            var categoryList = _db.categories.ToList();
            return categoryList;
        }

        [HttpGet("CategoryById/{id}")]
        public Category GetCategoryById(string id)
        {
            var categoryList = _db.categories.FirstOrDefault(e=>e.Id==id);
            return categoryList;
        }

        [HttpGet("ByCategory/{id}")]
        public async Task<List<Question>> GetQuestionByCategory(string id)
        {
            int idInt = Convert.ToInt32(id);
            if (idInt < 0 || idInt > 30)
            {
                return new List<Question>();
            }

            List<Question> questions;
            if (idInt == 0)
            {
                questions = await _db.questions
                    .Include(e => e.Category)
                    .Include(e => e.Answers).ThenInclude(e=>e.Ratings)
                    .Include(e => e.User).ThenInclude(e => e.Files)
                    .AsNoTracking().ToListAsync();
            }
            else
            {
                questions = await _db.questions
                    .Include(e => e.Category)
                    .Include(e => e.Answers).ThenInclude(e => e.Ratings)
                    .Include(e => e.User).ThenInclude(e => e.Files)
                    .Where(e => e.Category.Id == id).AsNoTracking().ToListAsync();
            }
           

            questions.ForEach(e => {
                e.Answers.ForEach(w => { w.Question = null; w.User = null; });
                e.Category.Questions = null;
                e.User.Answers = null;
                e.User.Questions = null;
                e.User.Files = new List<Models.File>() { e.User.Files.OrderByDescending(e => e.Date).FirstOrDefault() };
                e.User.Files.ForEach(e => e.User = null);
                e.User.Date = DateTime.Now;
                e.User.Email = null;
                e.User.NormalizedEmail = null;
                e.User.NormalizedUserName = null;
                e.User.PasswordHash = null;
                e.User.SecurityStamp = null;
                e.Answers.ForEach(e => e.Ratings.ForEach(e => e.Answer = null));
            });
      
            return questions;
        }
    }
}