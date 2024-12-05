using AutoMapper;
using Travelog.Core.Models;
using Travelog.DataAccess.Entities;
using Travelog.DataAccess.Models;

public class Mapper : Profile
{
    public Mapper()
    {
        CreateMap<UserEntity, User>()
            .ForMember(dest => dest.Places, opt => opt.MapFrom(src => src.Places));
        CreateMap<User,UserEntity>()
            .ForMember(dest => dest.Places, opt => opt.MapFrom(src => src.Places));
        
        CreateMap<Place, PlaceEntity>()
              .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos));
        CreateMap<PlaceEntity,Place> ()
              .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos));

        CreateMap<PhotoEntity, Photo>().ReverseMap();
    }
}
