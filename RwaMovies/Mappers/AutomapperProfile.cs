using AutoMapper;
using RwaMovies.DTOs;
using RwaMovies.Models;

namespace RwaMovies.Mappers
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<VideoRequest, Video>();
            CreateMap<Video, VideoResponse>()
                .ForMember(d => d.Tags, o => o.MapFrom(v => v.VideoTags.Select(vt => vt.Tag)));
            CreateMap<Genre, GenreDTO>();
            CreateMap<GenreDTO, Genre>();
            CreateMap<Image, ImageDTO>();
            CreateMap<Tag, TagDTO>();
            CreateMap<TagDTO, Tag>();
        }
    }
}
