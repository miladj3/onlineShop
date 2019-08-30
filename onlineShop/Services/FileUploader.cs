using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using onlineShop.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace onlineShop.Services
{
    public class FileUploader : IFileUploader
    {
        private const int maxCount = 10;
        private const int maxImgSizeMb = 1;
        private const string uploadCatalog = "uploads\\";

        private readonly IHostingEnvironment _env;

        public FileUploader(IHostingEnvironment env)
        {
            _env = env;
        }

        public bool ValidateImageSingle(IFormFile image, ref string errorMessage)
        {
            return ValidateImageMulti(new List<IFormFile> { image }, ref errorMessage);
        }

        public bool ValidateImageMulti(IList<IFormFile> images, ref string errorMessage)
        {
            var permittedExt = new List<String> { ".gif", ".jpg", ".jpeg", ".png" };

            if (images.Count > maxCount)
            {
                errorMessage = "Attachment number can't exceed " + maxCount + '.';
                return false;
            }

            foreach (var image in images)
            {
                if (!permittedExt.Contains(Path.GetExtension(image.FileName)))
                {
                    errorMessage = "Permitted extensions: " + string.Join(" ,", permittedExt.ToArray());
                    return false;
                }

                if (image.Length > (maxImgSizeMb * 1024 * 1024))
                {
                    errorMessage = "Attachment size can't exceed " + maxImgSizeMb + " MB.";
                    return false;
                }
            }

            return true;
        }

        public async Task<string> UploadSingle(IFormFile file)
        {
            return (await UploadMulti(new List<IFormFile> { file })).First();
        }

        public async Task<List<String>> UploadMulti(IList<IFormFile> files)
        {
            var filesUploaded = new List<String>();

            foreach (var image in files)
            {
                var uploadPath = Path.Combine(_env.WebRootPath, uploadCatalog);
                var fileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetFileName(image.FileName);
                var completePath = Path.Combine(uploadPath, fileName);
                var relPath = (Path.Combine(uploadCatalog, fileName));

                relPath = relPath.Replace(@"\", "/");

                using (var stream = new FileStream(completePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                filesUploaded.Add(relPath);
            }

            return filesUploaded;
        }
    }
}
