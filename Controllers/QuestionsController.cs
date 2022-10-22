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
    public class QuestionsController : ControllerBase
    {
        private UserManager<User> _userManager;
        private AuthentificationContext _db;
        public QuestionsController(UserManager<User> userManager, AuthentificationContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpGet]
        [Authorize]
        [Route("getquestionswithanswer")]
        public async Task<List<Question>> GetQuestionWithAnswerAuth()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);

            var questions = await _db.questions
              .Include(e => e.Category)
              .Include(e => e.Answers).ThenInclude(e=>e.Ratings)
              .Include(e => e.User).ThenInclude(e => e.Files)
             .Where(e => e.User.Id == user.Id).AsNoTracking().ToListAsync();

            questions.ForEach(e =>
            {
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

        [HttpGet("{id}")]
        public async Task<Question> GetQuestionWithAnswerAuthById(string id)
        {
            var questions = await _db.questions
                .Include(e => e.Files)
                .Include(e => e.Category)
                .Include(e => e.User).ThenInclude(e => e.Files)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            questions.User.Questions = null;
            questions.User.Date = DateTime.Now;
            questions.User.Email = null;
            questions.User.NormalizedEmail = null;
            questions.User.NormalizedUserName = null;
            questions.User.PasswordHash = null;
            questions.User.SecurityStamp = null;
            questions.User.Files = new List<Models.File>() { questions.User.Files.OrderByDescending(e => e.Date).FirstOrDefault() };
            questions.User.Files.ForEach(e => e.User = null);
            questions.Files.ForEach(e => e.Question = null);
            questions.Category.Questions = null;

            return questions;
        }

        [HttpGet("ById/{Userid}")]
        public async Task<List<Question>> GetQuestion(string Userid)
        {
            var questions = await _db.questions
                 .Include(e => e.Category)
                 .Include(e => e.Answers).ThenInclude(e=>e.Ratings)
                 .Include(e => e.User).ThenInclude(e => e.Files)
                 .AsNoTracking()
                 .Where(e => e.UserId == Userid).AsNoTracking().ToListAsync();

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

        [HttpPost]
        [Authorize]
        [Route("AddQuestion")]
        public async Task<IActionResult> SetQuestionProfileAuth()
        {
            IFormFileCollection file = Request.Form.Files;
            var a = Request.Form.FirstOrDefault().Value[0];
            var json = new JavaScriptSerializer().Deserialize<QuestionModel>(a);
            if (ModelState.IsValid)
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = await _userManager.FindByIdAsync(userId);
                var CategoryQuestion = _db.categories.FirstOrDefault(e => e.Id == json.Category);
                var questionItem = new Question()
                {
                    User = user,
                    UserId = user.Id,
                    Date = json.Date,
                    Head = json.Head,
                    Value = json.Value,
                    Category = CategoryQuestion,
                    CategoryId = CategoryQuestion.Id
                };
                _db.questions.Add(questionItem);

                foreach (var item in file)
                {
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(item.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)item.Length);
                    }
                    _db.files.Add(new Models.File()
                    {
                        FileName = item.FileName,
                        FileType = item.ContentType,
                        Date = DateTime.Now,
                        Value = imageData,
                        Question = questionItem,
                        QuestionId = questionItem.Id
                    });
                }
                await _db.SaveChangesAsync();
                return Ok( new { questionItem.Id });
            }
            return BadRequest(ModelState);
        }

        [HttpGet]
        [Route("GetLastQuestion")]
        public async Task<List<Question>> GetLastQuestion()
        {
            var questions = await _db.questions
                .Include(e => e.Category)
                .Include(e => e.Answers).ThenInclude(e=>e.Ratings)
                .Include(e => e.User).ThenInclude(e => e.Files)
                .OrderByDescending(e => e.Date).AsNoTracking().Take(10).ToListAsync();

            questions.ForEach(e =>
            {
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

        [HttpDelete("Delete/{id}")]
        public async Task<object> DeleteQuestion(string id)
        {
            var question = _db.questions.FirstOrDefault(e => e.Id == id);
            if (question != null)
            {
                var questionFile = await _db.files.Where(e => e.QuestionId == id).ToListAsync();
                var answer = await _db.answers.Where(e => e.QuestionId == id).ToListAsync();
                List<Rating> rating = new List<Rating>();
                List<Models.File> files = new List<Models.File>();
                foreach (var item in answer)
                {
                    rating = await _db.ratings.Where(e => e.AnswerId == item.Id).ToListAsync();
                    files = await _db.files.Where(e => e.AnswerId == item.Id).ToListAsync();
                    if (files.Count != 0)
                    {
                        foreach (var item1 in files)
                        {
                            _db.files.Remove(item1);
                        }
                    }
                    if (rating.Count != 0)
                    {
                        foreach (var item1 in rating)
                        {
                            _db.ratings.Remove(item1);
                        }
                    }
                }

                if (questionFile.Count != 0)
                {
                    foreach (var item in questionFile)
                    {
                        _db.files.Remove(item);
                    }
                }

                if (answer.Count != 0)
                {
                    foreach (var item in answer)
                    {
                        _db.answers.Remove(item);
                    }
                }
                _db.questions.Remove(question);
                await _db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }
    }
}