using AutoMapper;
using RwaMovies.Services;
using RwaMovies.Models.DAL;
using RwaMovies.Models.Shared.Auth;
using RwaMovies.Models.Shared;

namespace RwaMovies.Mappers
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<VideoRequest, Video>()
                .ForMember(d => d.CreatedAt, o => o.MapFrom((u, d) =>
                    d.CreatedAt == DateTime.MinValue ? DateTime.UtcNow : d.CreatedAt));
            CreateMap<Video, VideoResponse>()
                .ForMember(d => d.Tags, o => o.MapFrom(v => v.VideoTags.Select(vt => vt.Tag)));
            CreateMap<VideoResponse, VideoRequest>()
                .ForMember(d => d.GenreId, o => o.MapFrom(v => v.Genre.Id))
                .ForMember(d => d.TagIds, o => o.MapFrom(v => v.Tags.Select(t => t.Id)));
            CreateMap<Genre, GenreDTO>();
            CreateMap<GenreDTO, Genre>();
            CreateMap<Tag, TagDTO>();
            CreateMap<TagDTO, Tag>();
            CreateMap<Country, CountryDTO>();
            CreateMap<CountryDTO, Country>();
            CreateMap<NotificationRequest, Notification>()
                .ForMember(d => d.CreatedAt, o => o.MapFrom((u, d) => 
                    d.CreatedAt == DateTime.MinValue ? DateTime.UtcNow : d.CreatedAt));
            CreateMap<UserRequest, User>()
                .ForMember(d => d.CreatedAt, o => o.MapFrom((u, d) =>
                    d.CreatedAt == DateTime.MinValue ? DateTime.UtcNow : d.CreatedAt))
                .ForMember(d => d.Username, o => o.MapFrom(u => u.Username.Trim()))
                .ForMember(d => d.PwdSalt, o =>
                {
                    o.PreCondition(u => u.Password1 != "keep-password");
                    o.MapFrom(u => Convert.ToBase64String(AuthUtils.GenerateSalt()));
                })
                .ForMember(d => d.PwdHash, o =>
                {
                    o.PreCondition(u => u.Password1 != "keep-password");
                    o.MapFrom((u, d) => Convert.ToBase64String(AuthUtils.HashPassword(u.Password1, Convert.FromBase64String(d.PwdSalt))));
                })
                .ForMember(d => d.SecurityToken, o =>
                {
                    o.PreCondition(u => u.Password1 != "keep-password");
                    o.MapFrom(u => Convert.ToBase64String(AuthUtils.GenerateSecurityToken()));
                });
            CreateMap<User, UserResponse>()
                .ForMember(d => d.CountryOfResidence, o => o.MapFrom(u => u.CountryOfResidence.Name));
            CreateMap<User, UserRequest>();
        }
    }
}
