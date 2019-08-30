using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace onlineShop.Contracts
{
    public interface IFileUploader
    {
        Task<List<string>> UploadMulti(IList<IFormFile> files);
        Task<string> UploadSingle(IFormFile file);
        bool ValidateImageMulti(IList<IFormFile> images, ref string errorMessage);
        bool ValidateImageSingle(IFormFile image, ref string errorMessage);
    }
}