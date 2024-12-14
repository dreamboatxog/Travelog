using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travelog.Core.Abstractions;
using Travelog.Core.Models;
using Travelog.DataAccess.Entities;

namespace Travelog.DataAccess.Repositories
{
    public class PlacesRepository : IPlacesRepository
    {
        private readonly TravelogDbContext _context;
        private readonly IMapper _mapper;

        public PlacesRepository(TravelogDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<Place> AddAsync(Place place)
        {
            var placeEntity = _mapper.Map<PlaceEntity>(place);
            foreach (var photo in placeEntity.Photos)
            {
                photo.PlaceId = placeEntity.Id;
                photo.Place = placeEntity;
            }
            _context.Places.Add(placeEntity);
            await _context.SaveChangesAsync();
            return place;
        }

        public async Task<Place?> GetByIdAsync(Guid id)
        {

            var placeEntity = await _context.Places
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (placeEntity == null) return null;

            return _mapper.Map<Place>(placeEntity);
        }

        public async Task<bool> UpdateAsync(Place place)
        {
            var placeEntity = await _context.Places.Include(p => p.Photos).FirstOrDefaultAsync(p => p.Id == place.Id);
            if (placeEntity == null) 
                return false;
            placeEntity.Name = place.Name;
            placeEntity.Description = place.Description;
            placeEntity.Latitude = place.Latitude;
            placeEntity.Longitude = place.Longitude;

            // Синхронизация фотографий
            var existingPhotos = placeEntity.Photos.ToDictionary(p => p.Id);
            var updatedPhotos = place.Photos.ToDictionary(p => p.Id);

            // Удаляем фотографии, которых больше нет в обновлённой версии
            foreach (var photoToRemove in existingPhotos.Keys.Except(updatedPhotos.Keys))
            {
                var photoEntity = placeEntity.Photos.First(p => p.Id == photoToRemove);
                placeEntity.Photos.Remove(photoEntity);
            }

            // Добавляем новые фотографии
            foreach (var photoToAdd in updatedPhotos.Keys.Except(existingPhotos.Keys))
            {
                var photoEntity = _mapper.Map<PhotoEntity>(updatedPhotos[photoToAdd]);
                photoEntity.PlaceId = placeEntity.Id;
                photoEntity.Place = placeEntity;
                _context.Photos.Add(photoEntity);
                placeEntity.Photos.Add(photoEntity);

            }

            // Обновляем описания существующих фотографий
            foreach (var photoToUpdate in existingPhotos.Keys.Intersect(updatedPhotos.Keys))
            {
                var existingPhoto = placeEntity.Photos.First(p => p.Id == photoToUpdate);
                    existingPhoto.Description = updatedPhotos[photoToUpdate].Description;
            }


            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var placeEntity = await _context.Places.Include(p => p.Photos).FirstOrDefaultAsync(p => p.Id == id);
            if (placeEntity == null) return false;

            // Удаляем файлы с диска
            foreach (var photo in placeEntity.Photos)
            {
                var fileUrl = photo.FilePath; 
                var fileName = fileUrl.Split('/').Last(); 
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName); 

                if (File.Exists(filePath))
                {
                    File.Delete(filePath); 
                }
            }

            _context.Places.Remove(placeEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Place>?> GetByUserIdAsync(Guid userId)
        {
            var userPlacesEntities = await _context.Places
                                   .Where(p => p.UserId == userId)
                                   .Include(p => p.Photos) 
                                   .ToListAsync();
            if (userPlacesEntities == null)
            {
                return null;
            }
            List<Place> places = new List<Place>();
            foreach (var placeEntity in userPlacesEntities)
            {
                places.Add(_mapper.Map<Place>(placeEntity));
            }

            return places;
        }
    }
}
