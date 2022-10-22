using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Hubs;
using WebApplication2.Models;
using WebApplication2.Redis;
using WebApplication2.service;

namespace WebApplication2.Controllers
{
 
    [Route("[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private AuthentificationContext _db;
        private RedisService redisController;
        private UserManager<User> _userManager;
        public SubscriptionController(AuthentificationContext db, UserManager<User> userManager, IEasyCachingProviderFactory cachingProviderFactory)
        {
            redisController = new RedisService(cachingProviderFactory);
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("getsublist")]
        [Authorize]
        public async Task<List<Subscription>> Message()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            var temp = _db.subscriptions.Where(e => e.UserId == user.Id).ToList();
            temp.ForEach(e => e.User = null);
            return  temp;
        }

        [HttpPost]
        [Route("Sub")]
        [Authorize]
        public async Task<IActionResult> SetSub(Subscription model)
        {
            if (ModelState.IsValid)
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = await _userManager.FindByIdAsync(userId);
                var sub = await _db.subscriptions.FirstOrDefaultAsync(e => e.User_Sub == model.User_Sub && e.UserId == user.Id);
                if (sub == null)
                {
                    model.User = user;
                    _db.subscriptions.Add(model);
                    _db.SaveChanges();
                    model.User = null;
                    return Ok(model);
                }
                else
                {
                    _db.subscriptions.Remove(sub);
                    await _db.SaveChangesAsync();
                    return Ok();
                }
            }
            return BadRequest(ModelState);
        }

        [HttpGet("CropsSub")]
        [Authorize]
        public async Task<List<AppUserModel>> GetSubUserCrop()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            ImageService imageService = new ImageService(redisController, _db);

            List<User> subUser=new List<User>();
            foreach (var item in await _db.subscriptions.Where(e => e.UserId == userId).ToListAsync())
            {
                subUser.AddRange(_db.users.Include(e => e.Files).Include(e => e.subscriptions).Include(e=>e.Answers).ThenInclude(e=>e.Ratings).ToList().Where(e => e.Id == item.User_Sub));

            }
            imageService.FindAndTakeCrop(subUser, 200);

            return await imageService.GetCropImage(subUser, userId);
        }

        [HttpGet("CropsSubById/{id}")]
        public async Task<List<AppUserModel>> GetSubUserCropById(string id)
        {
            ImageService imageService = new ImageService(redisController, _db);

            List<User> subUser = new List<User>();
            foreach (var item in await _db.subscriptions.Where(e => e.UserId == id).ToListAsync())
            {
                subUser.AddRange(_db.users.Include(e => e.Files).Include(e => e.subscriptions).Include(e => e.Answers).ThenInclude(e => e.Ratings).ToList().Where(e => e.Id == item.User_Sub));

            }
            imageService.FindAndTakeCrop(subUser, 200);

            return await imageService.GetCropImage(subUser, id);
        }
    }
}