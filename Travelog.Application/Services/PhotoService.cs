using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Travelog.Core.Abstractions;

namespace Travelog.Application.Services
{
    public class PhotoService : IPhotosService
    {
        public async Task<string> SavePhotoAsync(IFormFile file)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine("C:\\Travelog.Photos", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

        public async Task<string> SavePhotoAsyncLocal(IFormFile file, string baseUrl)
        {
            // Путь для сохранения изображения на сервере
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Генерация уникального имени для файла
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            // Сохранение файла на сервере
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Возвращаем URL для доступа к файлу
            return baseUrl+"/images/" + fileName;
        }

        public Result<string> GetPhotoAsBase64(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return Result.Failure<string>("Файл не найден!");
            }

            // Читаем файл в массив байт
            byte[] fileBytes = File.ReadAllBytes(filePath);

            // Преобразуем в строку Base64
            string base64String = Convert.ToBase64String(fileBytes);

            return Result.Success<string>(base64String);
        }

        public Result<FileStream> GetPhotoAsFormFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return Result.Failure<FileStream>("Файл не найден!");
            }
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return Result.Success<FileStream>(fileStream);
        }

        /*public Result<IFormFile> GetPhotoAsFormFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return Result.Failure<IFormFile>("Файл не найден!"); 
            }
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var formFile = new FormFile(fileStream, 0, fileStream.Length, null, Path.GetFileName(filePath))
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg" 
            };
            return Result.Success<IFormFile>(formFile);
        }*/
    }

    
}
