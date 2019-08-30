using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using onlineShop.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace onlineShop.Controllers.API
{
    [ApiController]
    [Authorize(Roles = "Admin, Manager")]
    public class ControlPanelController : Controller
    {
        private readonly IFileUploader _uploader;

        public ControlPanelController(IFileUploader uploader)
        {
            _uploader = uploader;
        }

        [HttpPost("/ControlPanel/FileUpload/")]
        public async Task<IActionResult> UploadImages(IList<IFormFile> files)
        {
            string errorMessage = "";

            if (!_uploader.ValidateImageMulti(files, ref errorMessage))
                return BadRequest("Invalid attachment(s). " + errorMessage);

            var list = await _uploader.UploadMulti(files);

            return Json(list);
        }

    }
}
