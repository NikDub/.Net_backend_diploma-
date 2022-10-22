using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AuthentificationContext _db;
        public AdminController(UserManager<User> userManager, AuthentificationContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpDelete("DeleteAnswerByAdmin/{id}")]
        public async Task<object> DeleteAnswerByAdmin(string id)
        {
            var answer = await _db.Answers.FirstOrDefaultAsync(e => e.Id == id);
            if (answer != null)
            {
                var rating = await _db.Ratings.Where(e => e.AnswerId == id).ToListAsync();
                var files = await _db.Files.Where(e => e.AnswerId == id).ToListAsync();
                var complaints = await _db.Complaints.Where(e => e.AnswerId == id).ToListAsync();
                if (files.Count != 0)
                {
                    _db.Files.RemoveRange(files);
                }
                if (rating.Count != 0)
                {
                    _db.Ratings.RemoveRange(rating);
                }
                if (complaints.Count != 0)
                {
                    _db.Complaints.RemoveRange(complaints);
                }
                var not = await _db.Notifications.FirstOrDefaultAsync(e => e.AnswerId == answer.Id);
                _db.Notifications.Remove(not);
                _db.Answers.Remove(answer);
                await _db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("DeleteQuestionByAdmin/{id}")]
        public async Task<object> DeleteQuestionByAdmin(string id)
        {
            var question = await _db.Questions.FirstOrDefaultAsync(e => e.Id == id);
            if (question != null)
            {
                var questionFile = await _db.Files.Where(e => e.QuestionId == id).ToListAsync();
                var answer = await _db.Answers.Where(e => e.QuestionId == id).ToListAsync();
                var complaintsQuestions = await _db.Complaints.Where(e => e.QuestionId == question.Id).ToListAsync();
                List<Complaints> complaintsAnswers = new List<Complaints>();
                List<Rating> rating = new List<Rating>();
                List<Notifications> notif = new List<Notifications>();
                List<Models.File> files = new List<Models.File>();
                foreach (var item in answer)
                {
                    rating = await _db.Ratings.Where(e => e.AnswerId == item.Id).ToListAsync();
                    files = await _db.Files.Where(e => e.AnswerId == item.Id).ToListAsync();
                    notif = await _db.Notifications.Where(e => e.AnswerId == item.Id).ToListAsync();
                    complaintsAnswers = await _db.Complaints.Where(e => e.AnswerId == item.Id).ToListAsync();
                    if (files.Count != 0)
                        _db.Files.RemoveRange(files);
                    if (rating.Count != 0)
                        _db.Ratings.RemoveRange(rating);
                    if (notif.Count != 0)
                        _db.Notifications.RemoveRange(notif);
                    if (complaintsAnswers.Count != 0)
                        _db.Complaints.RemoveRange(complaintsAnswers);
                }

                if (questionFile.Count != 0)
                    _db.Files.RemoveRange(questionFile);

                if (answer.Count != 0)
                    _db.Answers.RemoveRange(answer);

                if (complaintsQuestions.Count != 0)
                    _db.Complaints.RemoveRange(complaintsQuestions);

                var not = await _db.Notifications.FirstOrDefaultAsync(e => e.QuestionId == question.Id);
                _db.Notifications.Remove(not);
                _db.Questions.Remove(question);
                await _db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("DeleteUserByAdmin/{id}")]
        public async Task<object> DeleteUserByAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.RemoveFromRolesAsync(user, new List<string>() { "Admin", "Customer" });

                var questions = await _db.Questions.Where(e => e.UserId == id).ToListAsync();
                var answers = await _db.Answers.Where(e => e.UserId == id).ToListAsync();
                var recalls = await _db.Recalls.Where(e => e.UserId == id).ToListAsync();
                var subscriptions = await _db.Subscriptions.Where(e => e.UserId == id).ToListAsync();
                var complaints = await _db.Complaints.Where(e => e.UserId == id).ToListAsync();
                var notifications = await _db.Notifications.Where(e => e.UserId == id).ToListAsync();
                var filesUser = await _db.Files.Where(e => e.UserId == id).ToListAsync();
                var ratings = await _db.Ratings.Where(e => e.UserId == id).ToListAsync();

                if (ratings.Count != 0)
                {
                    _db.Ratings.RemoveRange(ratings);
                }
                if (recalls.Count != 0)
                {
                    _db.Recalls.RemoveRange(recalls);
                }
                if (filesUser.Count != 0)
                {
                    _db.Files.RemoveRange(filesUser);
                }
                if (notifications.Count != 0)
                {
                    _db.Notifications.RemoveRange(notifications);
                }
                if (complaints.Count != 0)
                {
                    _db.Complaints.RemoveRange(complaints);
                }
                if (subscriptions.Count != 0)
                {
                    _db.Subscriptions.RemoveRange(subscriptions);
                }
                if (answers.Count != 0)
                {
                    List<Rating> ratingAnswers = new List<Rating>();
                    List<Notifications> notificationsAnswers = new List<Notifications>();
                    List<Models.File> filesAnswers = new List<Models.File>();
                    List<Complaints> complaintsAnswers = new List<Complaints>();
                    foreach (var item in answers)
                    {
                        ratingAnswers = await _db.Ratings.Where(e => e.AnswerId == item.Id).ToListAsync();
                        filesAnswers = await _db.Files.Where(e => e.AnswerId == item.Id).ToListAsync();
                        notificationsAnswers = await _db.Notifications.Where(e => e.AnswerId == item.Id).ToListAsync();
                        complaintsAnswers = await _db.Complaints.Where(e => e.AnswerId == item.Id).ToListAsync();
                        if (filesAnswers.Count != 0)
                        {
                            _db.Files.RemoveRange(filesAnswers);
                        }
                        if (ratingAnswers.Count != 0)
                        {
                            _db.Ratings.RemoveRange(ratingAnswers);
                        }
                        if (notificationsAnswers.Count != 0)
                        {
                            _db.Notifications.RemoveRange(notificationsAnswers);
                        }
                        if (complaintsAnswers.Count != 0)
                        {
                            _db.Complaints.RemoveRange(complaintsAnswers);
                        }
                    }
                    _db.Answers.RemoveRange(answers);

                }
                if (questions.Count != 0)
                {
                    foreach (var item in questions)
                    {
                        var questionFile = await _db.Files.Where(e => e.QuestionId == item.Id).ToListAsync();
                        var answer = await _db.Answers.Where(e => e.QuestionId == item.Id).ToListAsync();
                        var complaintsQuestions = await _db.Complaints.Where(e => e.QuestionId == item.Id).ToListAsync(); 
                        List<Rating> rating = new List<Rating>();
                        List<Notifications> notif = new List<Notifications>();
                        List<Complaints> complaintsAnswers = new List<Complaints>();
                        List<Models.File> files = new List<Models.File>();
                        foreach (var itemA in answer)
                        {
                            rating = await _db.Ratings.Where(e => e.AnswerId == itemA.Id).ToListAsync();
                            files = await _db.Files.Where(e => e.AnswerId == itemA.Id).ToListAsync();
                            notif = await _db.Notifications.Where(e => e.AnswerId == itemA.Id).ToListAsync();
                            complaintsAnswers = await _db.Complaints.Where(e => e.AnswerId == item.Id).ToListAsync();
                            if (files.Count != 0)
                            {
                                _db.Files.RemoveRange(files);
                            }
                            if (rating.Count != 0)
                            {
                                _db.Ratings.RemoveRange(rating);
                            }
                            if (notif.Count != 0)
                            {
                                _db.Notifications.RemoveRange(notif);
                            }
                            if (complaintsAnswers.Count != 0)
                            {
                                _db.Complaints.RemoveRange(complaintsAnswers);
                            }
                        }

                        if (questionFile.Count != 0)
                        {
                            _db.Files.RemoveRange(questionFile);
                        }

                        if (answer.Count != 0)
                        {
                            _db.Answers.RemoveRange(answer);
                        }
                        if (complaintsQuestions.Count != 0)
                        {
                            _db.Complaints.RemoveRange(complaintsQuestions);
                        }

                        var not = await _db.Notifications.FirstOrDefaultAsync(e => e.QuestionId == item.Id);
                        _db.Notifications.Remove(not);
                    }
                    _db.Questions.RemoveRange(questions);
                }

                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

        [HttpPatch("EditUser")]
        public async Task<object> EditUser(AppUserModel model)
        {
            var user = await _db.Users.FirstOrDefaultAsync(e => e.Id == model.Id);
            if (user != null)
            {
                user.Name = model.Name;
                user.UserName = model.UserName;
                await _userManager.UpdateAsync(user);
                return Ok();
            }
            return BadRequest();
        }
    }
}