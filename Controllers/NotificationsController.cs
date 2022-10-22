using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AuthentificationContext _db;
        public NotificationsController(UserManager<User> userManager, AuthentificationContext db)
        {
            _userManager = userManager;
            _db = db;
        }
        [HttpGet("GetAnswers")]
        public async Task<List<Notifications>> GetAnswers()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);

            var notificationAnswer = await _db.Notifications.AsNoTracking().AsQueryable().Where(e => e.Type == "Answer" && e.UserId == userId && e.Seen != true).ToListAsync();
            foreach (var e in notificationAnswer)
            {
                e.User = await _db.Users.Include(e => e.Files).AsNoTracking().AsQueryable().FirstOrDefaultAsync(ex => ex.Id == e.UserSubId);
                e.User.Notifications = null;
                e.User.Files = new List<Models.File>() { e.User.Files.AsQueryable().OrderByDescending(e => e.Date).FirstOrDefault() };
                e.User.Files.ForEach(e => e.User = null);
                e.Answer = await _db.Answers.AsNoTracking().AsQueryable().FirstOrDefaultAsync(ex => ex.Id == e.AnswerId);
                e.Answer.Notifications = null;
                e.Answer.User = null;
            }
            return notificationAnswer;
        }

        [HttpGet("GetRating")]
        public async Task<List<Notifications>> GetRating()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);

            var notificationRating = await _db.Notifications.AsNoTracking().AsQueryable().Where(e => e.Type == "Rating" && e.UserId == userId && e.Seen != true).ToListAsync();
            foreach (var e in notificationRating)
            {
                e.User = await _db.Users.Include(e => e.Files).AsNoTracking().AsQueryable().FirstOrDefaultAsync(ex => ex.Id == e.UserSubId);
                e.User.Notifications = null;
                e.User.Files = new List<Models.File>() { e.User.Files.OrderByDescending(e => e.Date).FirstOrDefault() };
                e.User.Files.ForEach(e => e.User = null);
                e.Answer = await _db.Answers.FirstOrDefaultAsync(ex => ex.Id == e.AnswerId);
                e.Answer.Notifications = null;
                e.Answer.User = null;
            }
            return notificationRating;
        }

        [HttpGet("GetSub")]
        public async Task<List<Notifications>> GetSub()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);


            var notificationSub = await _db.Notifications.AsNoTracking().AsQueryable().Where(e => e.Type == "Sub" && e.UserId == userId && e.Seen != true).ToListAsync();
            foreach (var e in notificationSub)
            {
                e.User = await _db.Users.Include(e => e.Files).AsNoTracking().AsQueryable().FirstOrDefaultAsync(ex => ex.Id == e.UserSubId);
                e.User.Notifications = null;
                e.User.Files = new List<Models.File>() { e.User.Files.OrderByDescending(e => e.Date).FirstOrDefault() };
                e.User.Files.ForEach(e => e.User = null);
            }
            return notificationSub;
        }


        [HttpGet("GetQuestion")]
        public async Task<List<Notifications>> GetQuestion()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);

            var subList = await _db.Subscriptions.AsNoTracking().AsQueryable().Where(e => e.UserId == user.Id).Select(e => new { e.User_Sub }).ToListAsync();

            List<Notifications> notificationQuestion = new List<Notifications>();
            foreach (var item in subList)
            {
                notificationQuestion.AddRange(await _db.Notifications.AsNoTracking().AsQueryable().Where(e => e.Type == "Question" && e.UserId == item.User_Sub && e.Seen != true).ToListAsync());
            }

            foreach (var e in notificationQuestion)
            {
                e.User = await _db.Users.Include(e => e.Files).AsNoTracking().AsQueryable().FirstOrDefaultAsync(ex => ex.Id == e.UserId);
                e.User.Notifications = null;
                e.User.Files = new List<Models.File>() { e.User.Files.OrderByDescending(e => e.Date).FirstOrDefault() };
                e.User.Files.ForEach(e => e.User = null);
            }

            return notificationQuestion;
        }


        [HttpGet("GetSeen")]
        public async Task<object> GetSeen()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);


            var notificationSub = await _db.Notifications.Where(e => e.UserId == userId && e.Seen != true).AsNoTracking().AsQueryable().ToListAsync();
            notificationSub.ForEach(e => e.Seen = true);
            _db.Notifications.UpdateRange(notificationSub);
            await _db.SaveChangesAsync();
            return Ok();
        }

    }
}