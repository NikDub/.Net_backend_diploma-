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
    public class AnswerController : ControllerBase
    {

        private UserManager<User> _userManager;
        private AuthentificationContext _db;
        public AnswerController(UserManager<User> userManager, AuthentificationContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpGet("{id}")]
        public async Task<List<Answer>> GetAnswerByQuestionId(string id)
        {
            var answers = await _db.answers
                .Include(e => e.User).ThenInclude(e => e.Files)
                .Include(e => e.Files)
                .Include(e => e.Ratings)
                .Where(e => e.QuestionId == id).AsNoTracking().ToListAsync();

            answers.ForEach(e =>
            {
                e.User.Answers = null;
                e.User.Date = DateTime.Now;
                e.User.Email = null;
                e.User.NormalizedEmail = null;
                e.User.NormalizedUserName = null;
                e.User.PasswordHash = null;
                e.User.SecurityStamp = null;
                e.User.Files = new List<Models.File>() { e.User.Files.OrderByDescending(e => e.Date).FirstOrDefault() };
                e.User.Files.ForEach(e => e.User = null);
                e.Files.ForEach(e => e.Answer = null);
                e.Ratings.ForEach(e => { e.User = null; e.Answer = null; });
            });
            return answers;
        }

        [HttpGet("Count/{id}")]
        public async Task<int> GetAnswerCountById(string id)
        {
            var count = await _db.questions.Where(e => e.Id == id).AsNoTracking().CountAsync();
            return count;
        }

        [HttpGet]
        [Route("GetLastAnswer")]
        public async Task<List<Answer>> GetLastAnswers()
        {
            var answers = await _db.answers
                .Include(e => e.User).ThenInclude(e => e.Files)
                .Include(e => e.Question).ThenInclude(e => e.Category)
                .OrderByDescending(e => e.Date).Take(10).AsNoTracking().ToListAsync();

            answers.ForEach(e =>
            {
                e.Question.Answers = null;
                e.Question.User = null;
                e.Question.Category.Questions = null;

                e.User.Files = new List<Models.File>() { e.User.Files.OrderByDescending(e => e.Date).FirstOrDefault() };
                e.User.Files.ForEach(e => e.User = null);
                e.User.Answers = null;
                e.User.Questions = null;
                e.User.Date = DateTime.Now;
                e.User.Email = null;
                e.User.NormalizedEmail = null;
                e.User.NormalizedUserName = null;
                e.User.PasswordHash = null;
                e.User.SecurityStamp = null;
            });
            return answers;
        }

        [HttpPost]
        [Authorize]
        [Route("AddAnswer")]
        public async Task<IActionResult> SetAnswerProfileAuth()
        {
            IFormFileCollection file = Request.Form.Files;
            var a = Request.Form.FirstOrDefault().Value[0];
            var json = new JavaScriptSerializer().Deserialize<AnswerModel>(a);
            if (ModelState.IsValid)
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = await _userManager.FindByIdAsync(userId);
                var question = await _db.questions.FirstOrDefaultAsync(e => e.Id == json.questionId);
                var answerItem = new Answer()
                {
                    Date = json.Date,
                    Value = json.Value,
                    User = user,
                    UserId = user.Id,
                    Question = question,
                    QuestionId = question.Id
                };
                _db.answers.Add(answerItem);

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
                        Answer = answerItem,
                        AnswerId = answerItem.Id
                    });
                }
                await _db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<object> DeleteAnswer(string id)
        {
            var answer = _db.answers.FirstOrDefault(e => e.Id == id);
            if (answer != null)
            {
                var rating = await _db.ratings.Where(e => e.AnswerId == id).ToListAsync();
                var files = await _db.files.Where(e => e.AnswerId == id).ToListAsync();
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

                _db.answers.Remove(answer);
                await _db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }
    }
}