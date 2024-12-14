using CSharpFunctionalExtensions;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;


namespace Travelog.Core.Models
{
    public class Place
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public Guid UserId { get; private set; } 
        public User User { get; private set; } = null!; 

        private List<Photo> _photos = new List<Photo>();

        public IReadOnlyCollection<Photo> Photos
        {
            get => _photos.AsReadOnly();
            private set => _photos = new List<Photo>(value);
        }

        public static Result<Place> Create(string name, string description, double latitude, double longitude, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
            {
                return Result.Failure<Place>("Название точки должно содержать не менее 3 символов.");
            }

            if (latitude < -90 || latitude > 90)
            {
                return Result.Failure<Place>("Широта должна быть в пределах от -90 до 90.");
            }

            if (longitude < -180 || longitude > 180)
            {
                return Result.Failure<Place>("Долгота должна быть в пределах от -180 до 180.");
            }
            if (userId == Guid.Empty)
            {
                return Result.Failure<Place>("Идентификатор пользователя не может быть пустым.");
            }

            var place = new Place
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                Latitude = latitude,
                Longitude = longitude,
                UserId = userId
            };

            return Result.Success(place);
        }

        public Result<bool> UpdateInfo(string name, string description, double latitude, double longitude)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
            {
                return Result.Failure<bool>("Название точки должно содержать не менее 3 символов.");
            }
            Name = name;
            Description = description;
            Latitude = latitude;
            Longitude = longitude;
            return Result.Success(true);
        }

        public Result AddPhoto(Photo photo)
        {
            if (_photos.Count > 5)
            {
                return Result.Failure("Максимально допустимое кол-во фотографий не может превышать 5!");
            }
            _photos.Add(photo);
            return Result.Success();
        }

        public void RemovePhoto(Photo photo)
        {
            _photos.Remove(photo);
        }

        public void RemoveAllPhotos()
        {
            _photos.Clear();
        }

    }

}

