using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HTML_FC.Controllers.Base
{
    [Route("api/v1/FileUpload/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly string urlSite;

        public FileController(IConfiguration configuration)
        {
            urlSite = configuration["urlSite"]!;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Upload System File")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadSystemFile(IFormFile file, string projectCode)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (!IsValidSystemFile(file))
                return BadRequest("Invalid file format or size.");



            string uploadDirectory = $"project2024/{projectCode.ToUpper()}";
            string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            string combinedUploadPath = Path.Combine(wwwRootPath, uploadDirectory);
            if (!Directory.Exists(combinedUploadPath))
            {
                try
                {
                    Directory.CreateDirectory(combinedUploadPath);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to create directory: {ex.Message}");
                }
            }

            string fileName = GenerateRandomString(12) + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(combinedUploadPath, fileName);

            if (System.IO.File.Exists(filePath))
            {
                string fileUrl = $"{urlSite}/{uploadDirectory}/{fileName}";
                return Ok();
            }

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string fileUrl = $"{urlSite}/{uploadDirectory}/{fileName}";
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        static string GenerateRandomString(int length)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost("Upload Single Picture")]
        public async Task<IActionResult> Upload(IFormFile file, string uploadDirectory)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (!IsValidFile(file))
                return BadRequest("Invalid file format or size.");

            if (string.IsNullOrEmpty(uploadDirectory))
            {
                return BadRequest("Upload directory is required.");
            }

            string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            string combinedUploadPath = Path.Combine(wwwRootPath, uploadDirectory);
            if (!Directory.Exists(combinedUploadPath))
            {
                try
                {
                    Directory.CreateDirectory(combinedUploadPath);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to create directory: {ex.Message}");
                }
            }

            string fileName = GenerateRandomString(12) + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(combinedUploadPath, fileName);

            if (System.IO.File.Exists(filePath))
            {
                string fileUrl = $"{urlSite}/{uploadDirectory}/{fileName}";
                return Ok(fileUrl);
            }

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string fileUrl = $"{urlSite}/{uploadDirectory}/{fileName}";
                return Ok(fileUrl);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Upload Multiple File")]
        public async Task<IActionResult> Upload(List<IFormFile> files, string uploadDirectory)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("No files uploaded.");
            }

            foreach (var file in files)
            {
                if (!IsValidFile(file))
                    return BadRequest($"Invalid file format or size for file: {file.FileName}");
            }

            if (string.IsNullOrEmpty(uploadDirectory))
            {
                return BadRequest("Upload directory is required.");
            }

            string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string combinedUploadPath = Path.Combine(wwwRootPath, uploadDirectory);


            if (!Directory.Exists(combinedUploadPath))
            {
                try
                {
                    Directory.CreateDirectory(combinedUploadPath);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to create directory: {ex.Message}");
                }
            }

            var uploadedFilesUrls = new List<string>();

            foreach (var file in files)
            {
                string fileName = GenerateRandomString(12) + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(combinedUploadPath, fileName);

                if (System.IO.File.Exists(filePath))
                {
                    string fileUrl = $"{urlSite}/{uploadDirectory}/{fileName}";
                    uploadedFilesUrls.Add(fileUrl);
                    continue;
                }

                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    string fileUrl = $"{urlSite}/{uploadDirectory}/{fileName}";
                    uploadedFilesUrls.Add(fileUrl);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"An error occurred: {ex.Message}");
                }
            }

            return Ok(uploadedFilesUrls);
        }


        private bool IsValidFile(IFormFile file)
        {
            const long maxFileSize = 10 * 1024 * 1024; // 10MB
            if (file.Length > maxFileSize)
                return false;

            string[] allowedMimeTypes = { "image/jpeg", "image/png", "image/gif", "application/pdf", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" };


            string contentType = file.ContentType.ToLowerInvariant();
            return allowedMimeTypes.Contains(contentType);
        }

        private bool IsValidSystemFile(IFormFile file)
        {
            const long maxFileSize = 20 * 1024 * 1024; // 10MB
            if (file.Length > maxFileSize)
                return false;

            string[] allowedMimeTypes = { "application/x-zip-compressed", "application/zip" };


            string contentType = file.ContentType.ToLowerInvariant();
            return allowedMimeTypes.Contains(contentType);
        }
    }
}
