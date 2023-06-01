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
			CreateMap<VideoResponse, VideoRequest>()
				.ForMember(d => d.GenreId, o => o.MapFrom(v => v.Genre.Id))
				.ForMember(d => d.TagIds, o => o.MapFrom(v => v.Tags.Select(t => t.Id)))
				.ForMember(d => d.ImageId, o => o.MapFrom(v => v.Image != null ? v.Image.Id : null));
			CreateMap<Genre, GenreDTO>();
			CreateMap<GenreDTO, Genre>();
			CreateMap<Image, ImageDTO>();
			CreateMap<Tag, TagDTO>();
			CreateMap<TagDTO, Tag>();
		}
	}
}
