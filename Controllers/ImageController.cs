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
using WebApplication2.Models;
using WebApplication2.Redis;
using WebApplication2.service;

namespace WebApplication2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private UserManager<User> _userManager;
        private RedisService redisController;
        private AuthentificationContext _db;
        public ImageController(UserManager<User> userManager, IEasyCachingProviderFactory cachingProviderFactory, AuthentificationContext db)
        {
            redisController = new RedisService(cachingProviderFactory);
            _userManager = userManager;
            _db = db;
        }

        [HttpPost]
        [Route("Upload")]
        public async Task<object> Upload()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);

            IFormFile file = Request.Form.Files.FirstOrDefault();

            if (file != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(file.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)file.Length);
                }
                Models.File file1 = new Models.File()
                {
                    Value = imageData,
                    FileName = file.FileName,
                    FileType = file.ContentType,
                    User = user,
                    Date = DateTime.Now
                };
                _db.files.Add(file1);
                await _db.SaveChangesAsync();
                var result = await _userManager.UpdateAsync(user);
                return Ok(result);
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("GetCrops")]
        public async Task<List<AppUserModel>> GetCrops()
        {
            var users = _userManager.Users.Include(e => e.Files).Include(e=>e.Answers).ThenInclude(e=>e.Ratings).ToList();
            string userId = null;
            try
            {
                userId = User.Claims.First(c => c.Type == "UserID").Value;
            }
            catch (Exception){}

            ImageService imageService = new ImageService(redisController, _db);
            imageService.FindAndTakeCrop(users, 200);
            return await imageService.GetCropImage(users, userId);
        }


        [HttpPost("Download")]
        public async Task<IActionResult> DownloadFile(Models.File fileId)
        {
            if (fileId.Id == null)
                return BadRequest();

            var file = _db.files.FirstOrDefault(e => e.Id == fileId.Id);
            var memory = new MemoryStream(file.Value);
            memory.Position = 0;
            var fileload = File(memory, file.FileType, file.FileName);
            return fileload;
        }
    }
}