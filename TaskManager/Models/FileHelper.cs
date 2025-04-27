using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TaskManager.Models
{
    public static class FileHelper
    {
        public static string SaveFile(IFormFile file, string uploadFolder = "uploads")
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }

            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".docx", ".xlsx", ".zip", ".rar" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!validExtensions.Contains(extension))
            {
                return null;
            }

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);


            var newFileName = fileNameWithoutExtension + "-" + Guid.NewGuid().ToString("N").Substring(0, 8) + extension;

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", uploadFolder);

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var filePath = Path.Combine(uploadPath, newFileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return newFileName;
            }
            catch
            {
                return null;
            }
        }



    }
}
