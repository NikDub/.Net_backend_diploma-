using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApplication2.Models;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApplication2.Redis;
using System;
using EasyCaching.Core;

namespace WebApplication2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private RedisService redisController;
        private UserManager<User> _userManager;
        private AuthentificationContext _db;
        public RatingsController(UserManager<User> userManager, IEasyCachingProviderFactory cachingProviderFactory, AuthentificationContext db)
        {
            _userManager = userManager;
            redisController = new RedisService(cachingProviderFactory);
            _db = db;
        }


        [HttpPost]
        [Authorize]
        public async Task<object> SetRatingById(Rating Id)
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            var ratingCheack = _db.ratings.FirstOrDefault(e => e.AnswerId == Id.Id && e.UserId == userId);
            if (ratingCheack != null)
            {
                _db.ratings.Remove(ratingCheack);
                await _db.SaveChangesAsync();
                return Ok("");
            }
            else
            {
                var rating = new Rating()
                {
                    Answer = await _db.answers.FirstAsync(e => e.Id == Id.Id),
                    AnswerId = Id.Id,
                    User = user,
                    UserId = userId,
                    Value = 1
                };
                _db.ratings.Add(rating); ;
                await _db.SaveChangesAsync();
                return Ok(new { });
            }
        }

        [HttpGet]
        public async Task<List<AppUserModel>> GetListRating()
        {
            var users = await _db.users.Include(e=>e.Answers).ThenInclude(e=>e.Ratings).Where(e=>e.Answers.Count!=0 && e.Answers.Any(e=>e.Ratings.Count!=0)).ToListAsync();

            var ratingList = new List<AppUserModel>();
            users.ForEach(e => 
            {
                var tenp = e.Answers.Select(er => er.Ratings.Count());
                var temp = new AppUserModel()
                {
                    Name = e.Name,
                    Id = e.Id,
                    UserName = e.UserName,
                    Image = redisController.getItem(e.Id),
                    RatingCount =tenp.Aggregate((x,y)=>x+y)
                };
                ratingList.Add(temp);
            });
           
            return ratingList.OrderByDescending(e=>e.RatingCount).Take(10).ToList();
        }
    }
}