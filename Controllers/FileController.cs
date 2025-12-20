using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [Route("api/files")]
    [ApiController]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public FileController(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        private string ResolvePath(string filePath)
        {
            filePath = filePath.Replace("\\", "/");

            string basePath = _config["StoragePath"] ?? "wwwroot/uploads";
            return Path.Combine(_env.ContentRootPath, basePath, filePath.TrimStart('/'));
        }

        private bool IsPathSafe(string filePath)
        {
            return !string.IsNullOrWhiteSpace(filePath)
                   && !filePath.Contains("..");
        }

        // ================= DOWNLOAD =================
        [HttpGet("download")]
        public IActionResult Download([FromQuery] string path)
        {
            if (!IsPathSafe(path))
                return BadRequest("Invalid file path.");

            var fullPath = ResolvePath(path);

            if (!System.IO.File.Exists(fullPath))
                return NotFound("File not found.");

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            return File(stream, "application/octet-stream", Path.GetFileName(fullPath));
        }

        // ================= IMAGE BASE64 =================
        [HttpGet("image")]
        public IActionResult ImageBase64([FromQuery] string path)
        {
            if (!IsPathSafe(path))
                return BadRequest("Invalid file path.");

            var fullPath = ResolvePath(path);

            if (!System.IO.File.Exists(fullPath))
                return NotFound("File not found.");

            var bytes = System.IO.File.ReadAllBytes(fullPath);
            var ext = Path.GetExtension(fullPath).ToLowerInvariant();

            var mime = ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            return Ok(new
            {
                fileName = Path.GetFileName(fullPath),
                mimeType = mime,
                dataUrl = $"data:{mime};base64,{Convert.ToBase64String(bytes)}"
            });
        }
    }
}
