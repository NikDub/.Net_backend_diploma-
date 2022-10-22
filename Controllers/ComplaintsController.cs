using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AuthentificationContext _db;
        public ComplaintsController(UserManager<User> userManager, AuthentificationContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpPost("SetAnswer")]
        [Authorize]
        public async Task<object> SetAnnswerComplain(Complaints model)
        {
            try
            {
                model.Answer = _db.Answers.FirstOrDefault(e => e.Id == model.AnswerId);
                _db.Complaints.Add(model);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }

        [HttpPost("SetQuestion")]
        [Authorize]
        public async Task<object> SetQuestionComplain(Complaints model)
        {
            try
            {

                model.Question = _db.Questions.FirstOrDefault(e => e.Id == model.QuestionId);
                _db.Complaints.Add(model);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }

        [HttpPost("SetUser")]
        [Authorize]
        public async Task<object> SetUserComplain(Complaints model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                model.User = user;
                _db.Complaints.Add(model);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }




        [HttpGet("GetComplainsAnswer")]
        [Authorize(Roles ="Admin")]
        public async Task<List<Complaints>> GetAnswer()
        {
            var comp = await _db.Complaints.Include(e=>e.Answer).ThenInclude(e=>e.Question).Where(e => e.Answer != null).ToListAsync();

            comp.ForEach(e =>
            {
                e.Answer.Complaints = null;
                e.Answer.Question.Answers = null;

            });

            return comp;
        } 
      
        [HttpGet("GetComplainsQuestion")]
        [Authorize(Roles ="Admin")]
        public async Task<List<Complaints>> GetQuestion()
        {
            var comp = await _db.Complaints.Include(e=>e.Question).Where(e => e.Question != null).ToListAsync();

            comp.ForEach(e =>
            {
                e.Question.Complaints = null;
            });

            return comp;
        }

        [HttpGet("GetComplainsUser")]
        [Authorize(Roles ="Admin")]
        public async Task<List<Complaints>> GetUser() 
        { 
            var comp = await _db.Complaints.Include(e => e.User).Where(e => e.Question == null && e.Answer==null).ToListAsync();

            comp.ForEach(e => {
                e.User.Date = DateTime.Now;
                e.User.Email = null;
                e.User.NormalizedEmail = null;
                e.User.NormalizedUserName = null;
                e.User.PasswordHash = null;
                e.User.SecurityStamp = null;
                e.User.Recalls = null;
                e.User.Complaints = null;
            });

            return comp;
        }



        [HttpPost("ById")]
        [Authorize(Roles ="Admin")]
        public async Task<object> SetSeen(Complaints model)
        {

            try
            {
                var compl = await _db.Complaints.FirstOrDefaultAsync(e=>e.Id==model.Id);
                compl.Seen = !compl.Seen;
                _db.Update(compl);
                await _db.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }

    }
}