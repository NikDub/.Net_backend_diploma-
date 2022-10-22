using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nancy.Json;
using WebApplication2.Models;
using WebApplication2.Models.DBModel;
using WebApplication2.Redis;
using WebApplication2.service;

namespace WebApplication2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private UserManager<User> _userManager;
        private RedisService redisController;
        private AuthentificationContext _db;
        public SearchController(UserManager<User> userManager, AuthentificationContext db, IEasyCachingProviderFactory cachingProviderFactory)
        {
            redisController = new RedisService(cachingProviderFactory);
            _userManager = userManager;
            _db = db;
        }

        [HttpGet("SearchByQuestion/{term}")]
        public async Task<List<Question>> SearchByQuestion(string term)
        {
            term = term.ToLower();
            var questions = _db.questions.Include(e => e.Answers).Include(e => e.Category).Where(e => e.Head.ToLower().Contains(term) || e.Value.ToLower().Contains(term)).AsNoTracking().ToList();
            questions.ForEach(e => e.Answers.ForEach(w => w.Question = null));
            questions.ForEach(e => e.User = null);
            questions.ForEach(e => e.Category.Questions = null);
            return questions;
        }

        [HttpGet("SearchByUser/{term}")]
        public async Task<List<AppUserModel>> SearchByUser(string term)
        {
            term = term.ToLower();
            var users = _userManager.Users
                .Include(e => e.Files)
                .Include(e=>e.Answers).ThenInclude(e=>e.Ratings)
                .Where(e=>e.Name.ToLower().Contains(term)||e.UserName.ToLower().Contains(term)).ToList();
            string userId = null;

            ImageService imageService = new ImageService(redisController, _db);
            imageService.FindAndTakeCrop(users, 200);
            var temp = await imageService.GetCropImage(users, userId);
            return  temp;
        }
    }
}