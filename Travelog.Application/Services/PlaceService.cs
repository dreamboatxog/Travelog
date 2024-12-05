﻿using CSharpFunctionalExtensions;
using Travelog.Core.Abstractions;
using Travelog.Core.Models;
using Travelog.Contracts;
using System.Collections.Generic;
using Travelog.Contracts.UpdatePlace;
using Travelog.Contracts.CreatePlace;

namespace Travelog.Application.Services
{
    public class PlaceService : IPlacesService
    {
        private readonly IPlacesRepository _placesRepository;
        private readonly IPhotosService _photosService;
        private readonly IUsersRepository _usersRepository;

        public PlaceService(IPlacesRepository placesRepository, IPhotosService photosService, IUsersRepository usersRepository)
        {
            _placesRepository = placesRepository;
            _photosService = photosService;
            _usersRepository = usersRepository;
        }

        public async Task<Result<Guid>> Create(Guid userId, PlaceCreateDTO placeDto, string baseUrl)
        {
            if (userId == Guid.Empty)
            {
                return Result.Failure<Guid>("Идентификатор пользователя не найден.");
            }
            var placeResult = Place.Create(placeDto.Name, placeDto.Description, placeDto.Latitude, placeDto.Longitude, userId);
            if (placeResult.IsFailure)
            {
                return Result.Failure<Guid>(placeResult.Error);
            }

            var place = placeResult.Value;


            foreach (var photoDto in placeDto.Photos)
            {
                var filePath = await _photosService.SavePhotoAsyncLocal(photoDto.File, baseUrl);
                var photoResult = Photo.Create(filePath, photoDto.Description);
                if (photoResult.IsFailure)
                {
                    return Result.Failure<Guid>(photoResult.Error);
                }

                place.AddPhoto(photoResult.Value);
            }

            var placeId = await _placesRepository.AddAsync(place);

            return Result.Success(placeId);
        }

        public async Task<Result<PlaceResponseDTO>> GetById(Guid id)
        {
            var place = await _placesRepository.GetByIdAsync(id);
            if (place == null)
                return Result.Failure<PlaceResponseDTO>("Place not found.");
            var placeResponse = new PlaceResponseDTO
            {
                Id = place.Id,
                Name = place.Name,
                Description = place.Description,
                Latitude = place.Latitude,
                Longitude = place.Longitude,
                Photos = place.Photos.Select(photo =>
                {
                    var fileUrl = Path.GetFileName(photo.FilePath);

                    return new PhotoResponseDTO
                    {
                        FilePath = fileUrl,
                        Description = photo.Description
                    };
                }).ToList()
            };

            return Result.Success<PlaceResponseDTO>(placeResponse);
        }

        public async Task<Result<List<PlaceResponseDTO>>> GetPlacesByUserId(Guid userId)
        {
            var places = await _placesRepository.GetByUserIdAsync(userId);

            if (places == null)
            {
                return Result.Failure<List<PlaceResponseDTO>>("Не найдено");
            }

            List<PlaceResponseDTO> placeResponses = new List<PlaceResponseDTO>();

            foreach (var place in places)
            {
                var placeResponse = new PlaceResponseDTO
                {
                    Id = place.Id,
                    Name = place.Name,
                    Description = place.Description,
                    Latitude = place.Latitude,
                    Longitude = place.Longitude,
                    Photos = place.Photos.Select(photo =>
                    {
                        return new PhotoResponseDTO
                        {
                            FilePath = photo.FilePath,
                            Description = photo.Description
                        };
                    }).ToList()
                };

                placeResponses.Add(placeResponse);
            }

            return Result.Success<List<PlaceResponseDTO>>(placeResponses);
        }

        public async Task<Result<List<PlaceResponseDTO>>> GetPlacesByUserName(string username)
        {
            var user = await _usersRepository.SearchUsersByUserNameAsync(username);
            var userId = user.FirstOrDefault()?.Id;
            if (userId == null)
            {
                return Result.Failure<List<PlaceResponseDTO>>("Пользователь с указанным юзернеймом не найден.");
            }
            var places = await _placesRepository.GetByUserIdAsync(userId.Value);
            if (places == null || !places.Any())
            {
                return Result.Failure<List<PlaceResponseDTO>>("Места не найдены.");
            }

            List<PlaceResponseDTO> placeResponses = new List<PlaceResponseDTO>();

            foreach (var place in places)
            {
                var placeResponse = new PlaceResponseDTO
                {
                    Id = place.Id,
                    Name = place.Name,
                    Description = place.Description,
                    Latitude = place.Latitude,
                    Longitude = place.Longitude,
                    Photos = place.Photos.Select(photo =>
                    {

                        return new PhotoResponseDTO
                        {
                            FilePath = photo.FilePath,
                            Description = photo.Description
                        };
                    }).ToList()
                };

                placeResponses.Add(placeResponse);
            }
            return Result.Success<List<PlaceResponseDTO>>(placeResponses);
        }

        public async Task<Result<PlaceResponseDTO>> UpdatePlaceAsync(Guid id, PlaceUpdateDTO placeUpdate, Guid userId, string baseUrl)
        {
            // Получаем место из репозитория
            var place = await _placesRepository.GetByIdAsync(id);

            if (place == null)
            {
                return Result.Failure<PlaceResponseDTO>("Точка не найдена.");
            }

            // Проверяем права пользователя
            if (place.UserId != userId)
            {
                return Result.Failure<PlaceResponseDTO>("Нет доступа для редактирования текущей точки.");
            }

            if(placeUpdate.Photos.Where(ap => string.IsNullOrWhiteSpace(ap.FilePath) && ap.File==null).Count()>0)
            {
                return Result.Failure<PlaceResponseDTO>("Свойства File и FilePath не могут быть одновременно пусты!");
            }

            if (placeUpdate.Photos.Where(ap => !string.IsNullOrWhiteSpace(ap.FilePath) && ap.File != null).Count() > 0)
            {
                return Result.Failure<PlaceResponseDTO>("Свойства File и FilePath не могут быть одновременно заполнены!");
            }
            // Обновляем основную информацию о месте
            place.UpdateInfo(placeUpdate.Name, placeUpdate.Description, placeUpdate.Latitude, placeUpdate.Longitude);

            // Обработка фотографий
            var photosToRemove = place.Photos
                .Where(p => !placeUpdate.Photos.Any(ep => ep.Id == p.Id))
                .ToList();

            var photosToAdd = placeUpdate.Photos
                .Where(ap => ap.File != null && string.IsNullOrWhiteSpace(ap.FilePath))
                .ToList();

            var photosToUpdate = placeUpdate.Photos
                .Where(ap => !string.IsNullOrWhiteSpace(ap.FilePath))
                .ToList();

            // Удаление старых фотографий
            foreach (var photoToRemove in photosToRemove)
            {
                place.RemovePhoto(photoToRemove);
            }

            // Добавление новых фотографий
            foreach (var photoToAdd in photosToAdd)
            {
                var filePath = await _photosService.SavePhotoAsyncLocal(photoToAdd.File, baseUrl);
                var photoResult = Photo.Create(filePath, photoToAdd.Description);

                if (photoResult.IsFailure)
                {
                    return Result.Failure<PlaceResponseDTO>(photoResult.Error);
                }

                place.AddPhoto(photoResult.Value);
            }

            // Обновление описаний существующих фотографий
            foreach (var photoToUpdate in photosToUpdate)
            {
                var existingPhoto = place.Photos.FirstOrDefault(p => p.Id == photoToUpdate.Id);

                if (existingPhoto != null)
                {
                    existingPhoto.UpdateDescription(photoToUpdate.Description);
                }
            }

            // Сохраняем изменения в репозитории
            await _placesRepository.UpdateAsync(place);

            
            return Result.Success(ToResponseDTO(place));
        }

        private PlaceResponseDTO ToResponseDTO(Place place)
        {
            return new PlaceResponseDTO
            {
                Id = place.Id,
                Name = place.Name,
                Description = place.Description,
                Latitude = place.Latitude,
                Longitude = place.Longitude,
                Photos = place.Photos.Select(p => new PhotoResponseDTO
                {
                    FilePath = p.FilePath,
                    Description = p.Description
                }).ToList()
            };
        }

    }
}