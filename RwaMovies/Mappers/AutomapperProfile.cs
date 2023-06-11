using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using RwaMovies.DTOs;
using RwaMovies.DTOs.Auth;
using RwaMovies.Models;
using System.Security.Cryptography;

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
                .ForMember(d => d.TagIds, o => o.MapFrom(v => v.Tags.Select(t => t.Id)));
            CreateMap<Genre, GenreDTO>();
            CreateMap<GenreDTO, Genre>();
            CreateMap<Tag, TagDTO>();
            CreateMap<TagDTO, Tag>();
            CreateMap<NotificationRequest, Notification>()
                .ForMember(d => d.CreatedAt, o => o.MapFrom(u => DateTime.UtcNow));
            CreateMap<RegisterRequest, User>()
                .ForMember(d => d.CreatedAt, o => o.MapFrom(u => DateTime.UtcNow))
                .ForMember(d => d.PwdSalt, o => o.MapFrom(u => Convert.ToBase64String(RandomNumberGenerator.GetBytes(128 / 8))))
                .ForMember(d => d.PwdHash, o => o.MapFrom((u, d) => Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: u.Password,
                    salt: Convert.FromBase64String(d.PwdSalt),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8))))
                .ForMember(d => d.SecurityToken, o => o.MapFrom(u => Convert.ToBase64String(RandomNumberGenerator.GetBytes(256 / 8))));
            CreateMap<User, RegisterResponse>();
        }
    }
}
