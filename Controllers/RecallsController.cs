using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecallsController : ControllerBase
    {
        private UserManager<User> _userManager;
        private AuthentificationContext _db;
        public RecallsController(UserManager<User> userManager, AuthentificationContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpGet]
        public async Task<List<Recalls>> GetRecalls()
        {
            var usersRecalls = await _db.Recalls.Include(e => e.User).ThenInclude(e => e.Files).ToListAsync();
            usersRecalls.ForEach(e => {
                e.User.Files = new List<Models.File>() { e.User.Files.OrderByDescending(e => e.Date).FirstOrDefault() };
                e.User.Files.ForEach(e => e.User = null);
                e.User.Date = DateTime.Now;
                e.User.Email = null;
                e.User.NormalizedEmail = null;
                e.User.NormalizedUserName = null;
                e.User.PasswordHash = null;
                e.User.SecurityStamp = null;
                e.User.Recalls = null;
            });
            return usersRecalls;
        }

        [HttpPost("Set")]
        [Authorize]
        public async Task<object> SetRecalls(Recalls model)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            try
            {
                model.User = user;
                model.UserId = userId;
                _db.Recalls.Add(model);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
         
            return Ok();
        }


        [HttpDelete("Delete/{id}")]
        public async Task<object> DeleteRecalls(string id)
        {
            var recall = await _db.Recalls.FirstOrDefaultAsync(e => e.Id == id);
            if (recall != null)
            {
                _db.Recalls.Remove(recall);
                await _db.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }

    }
}