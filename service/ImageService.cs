using EasyCaching.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;
using WebApplication2.Redis;

namespace WebApplication2.service
{
    public class ImageService : ImageToByteArrayService
    {
        private RedisService redisController;
        private AuthentificationContext _db;
        readonly byte[] defaultImage;
        public ImageService(RedisService cachingProviderFactory, AuthentificationContext db)
        {
            redisController = cachingProviderFactory;
            _db = db;
            defaultImage = ImageToByteArray(Image.FromFile(@"C:\Users\Никита Дубовский\source\repos\WebApplication2\WebApplication2\man.png"));
        }
            
        public async Task<List<AppUserModel>> GetCropImage(List<User> users, string userId)
        {
            List<AppUserModel> UserCropArray = new List<AppUserModel>();
            
            foreach (var item in users)//get crop for users
            {
                 if (item.Id == userId && userId!=null)
                    continue;
                var tenp = item.Answers.Select(e => e.Ratings.Count());
                var temp = new AppUserModel()
                {
                    Image = redisController.getItem(item.Id),
                    Id=item.Id,
                    Name=item.Name,
                    UserName= item.UserName,
                    QuestionCount = _db.questions.Where(e => e.UserId == item.Id ).Count(),
                    RatingCount = tenp.FirstOrDefault()
                };

                if (temp.Image == null)
                {
                    temp.Image = defaultImage;
                }
                UserCropArray.Add(temp);
            }
            return UserCropArray;
        }

        public async void FindAndTakeCrop(List<User> users, int size)
        {
            foreach (var item in users)///find users without crop
            {
                if (item.Files.Count != 0)
                {
                    redisController.setItem(item.Id, CropImage(size, users.FirstOrDefault(e=>e.Id== item.Id).Files?.OrderByDescending(e=>e.Date).FirstOrDefault().Value));
                }
            }
        }

    }

    public class ImageToByteArrayService
    {
        public ImageToByteArrayService()
        {}
        
        public byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public byte[] CropImage(int newSize, byte[] tmp)
        {
            var src = Image.FromStream(new MemoryStream(tmp));

            if (src.Width <= newSize)
                newSize = src.Width;

            var newHeight = src.Height * newSize / src.Width;

            if (newHeight > newSize)
            {
                // Resize with height instead
                newSize = src.Width * newSize / src.Height;
                newHeight = newSize;
            }

            return ImageToByteArray(src.GetThumbnailImage(newSize, newHeight, null, IntPtr.Zero));
        }

    }
}
