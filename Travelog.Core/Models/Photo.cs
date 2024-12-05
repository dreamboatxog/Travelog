using CSharpFunctionalExtensions;

namespace Travelog.Core.Models
{
    public class Photo
    {
        public Guid Id { get; private set; }
        public string FilePath { get; private set; }
        public string Description { get; private set; }

        private Photo() { }

        public static Result<Photo> Create(string filePath, string description)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return Result.Failure<Photo>("Путь к файлу не может быть пустым.");
            }

            var photo = new Photo
            {
                Id = Guid.NewGuid(),
                FilePath = filePath,
                Description = description
            };

            return Result.Success(photo);
        }

        public void UpdateDescription(string description)
        {
            Description = description;
        }
    }

}