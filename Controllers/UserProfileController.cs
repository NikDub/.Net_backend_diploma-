using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;
using WebApplication2.service;

namespace WebApplication2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private UserManager<User> _userManager;
        private AuthentificationContext _db;
        public UserProfileController(UserManager<User> userManager, AuthentificationContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpGet]
        [Authorize]
        public async Task<object> GetUserProfile()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var filecheck = _db.files.Where(e => e.User.Id == userId).ToList();
            Models.User user;
            if (filecheck.Count != 0)
            {
                user = _db.users.Include(e => e.Files).Where(e => e.Id == userId).FirstOrDefault();
                return new
                {
                    user.Name,
                    user.Email,
                    user.UserName,
                    user.Files?.OrderByDescending(e => e.Date).FirstOrDefault().Value,
                    user.Id
                };
            }
            else
            {
                user = _db.users.Where(e => e.Id == userId).FirstOrDefault();
                return new
                {
                    user.Name,
                    user.Email,
                    user.UserName,
                    user.Id
                };
            }
        }


        [HttpGet("GetUserId")]
        public async Task<object> GetUserId()
        {
            string Id = User.Claims.First(c => c.Type == "UserID").Value;
            return Ok(new { Id });
        }

        [HttpGet("{id}")]
        public async Task<AppUserModel> GetUserProfileById(string id)
        {
            var user = _db.users.Include(e=>e.Files).FirstOrDefault(e=>e.Id==id);
            AppUserModel appUserModel = new AppUserModel()
            {
                Name = user.Name,
                Email = user.Email,
                UserName = user.UserName,
                Id = user.Id
            };

            if (user.Files.Count == 0)
            {
                appUserModel.Image = new ImageToByteArrayService().ImageToByteArray(Image.FromFile(@"C:\Users\Никита Дубовский\source\repos\WebApplication2\WebApplication2\man.png"));
            }
            else
            {
                appUserModel.Image = user.Files.OrderByDescending(e => e.Date).FirstOrDefault().Value;
            }
            return appUserModel;
        }
    }
}