using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RwaMovies.DTOs;
using RwaMovies.Exceptions;
using RwaMovies.Models;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace RwaMovies.Services
{
    public interface IVideosService
    {
        Task<IEnumerable<VideoResponse>> GetVideos();
        Task<VideoResponse> GetVideo(int id);
        Task<IEnumerable<VideoResponse>> Search(
                       string? nameFilter, string? orderBy, string? orderDirection, int? page, int? size);
        Task PutVideo(int id, VideoRequest videoRequest);
        Task<int> PostVideo(VideoRequest videoRequest);
        Task DeleteVideo(int id);
    }

    public class VideosService : IVideosService
    {
        private readonly RwaMoviesContext _context;
        private readonly IMapper _mapper;

        public VideosService(RwaMoviesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VideoResponse>> GetVideos()
        {
            var videos = await _context.Videos
                .Include(v => v.Genre)
                .Include(v => v.Image)
                .Include(v => v.VideoTags).ThenInclude(vt => vt.Tag)
                .ToListAsync();
            return _mapper.Map<List<VideoResponse>>(videos);
        }

        public async Task<VideoResponse> GetVideo(int id)
        {
            var video = await _context.Videos
                .Where(v => v.Id == id)
                .Include(v => v.Genre)
                .Include(v => v.Image)
                .Include(v => v.VideoTags).ThenInclude(vt => vt.Tag)
                .FirstOrDefaultAsync();
            if (video is null)
                throw new NotFoundException();
            return _mapper.Map<VideoResponse>(video);
        }

        public async Task<IEnumerable<VideoResponse>> Search(
            string? nameFilter, string? orderBy, string? orderDirection, int? page, int? size)
        {
            orderBy ??= "Id";
            orderDirection ??= "asc";
            page ??= 1;
            size ??= 5;
            if (!new[] { "Id", "Name", "TotalSeconds" }.Contains(orderBy) ||
                !new[] { "asc", "desc" }.Contains(orderDirection) || page < 1 || size < 1)
                throw new BadRequestException();
            var videos = await _context.Videos
                .Where(v => v.Name.Contains(nameFilter ?? ""))
                .OrderBy($"{orderBy} {orderDirection}")
                .Skip((int)((page - 1) * size))
                .Take((int)size)
                .Include(v => v.Genre)
                .Include(v => v.Image)
                .Include(v => v.VideoTags).ThenInclude(vt => vt.Tag)
                .ToListAsync();
            return _mapper.Map<List<VideoResponse>>(videos);
        }

        public async Task PutVideo(int id, VideoRequest videoRequest)
        {
            try
            {
                var video = _mapper.Map<Video>(videoRequest);
                var existingVideoTags = await _context.VideoTags.Where(vt => vt.VideoId == id).ToListAsync();
                var newVideoTags = videoRequest.TagIds.Select(x => new VideoTag { VideoId = id, TagId = x });
                _context.VideoTags.RemoveRange(existingVideoTags);
                _context.VideoTags.AddRange(newVideoTags);
                _context.Entry(video).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch
            {
                if (!VideoExists(id))
                    throw new NotFoundException();
                throw;
            }
        }

        public async Task<int> PostVideo(VideoRequest videoRequest)
        {
            var video = _mapper.Map<Video>(videoRequest);
            video.VideoTags = videoRequest.TagIds.Select(x => new VideoTag { TagId = x }).ToList();
            _context.Videos.Add(video);
            await _context.SaveChangesAsync();
            return video.Id;
        }

        public async Task DeleteVideo(int id)
        {
            var video = await _context.Videos.Include(v => v.VideoTags).FirstOrDefaultAsync(v => v.Id == id);
            if (video == null)
                throw new NotFoundException();
            _context.Videos.Remove(video);
            await _context.SaveChangesAsync();
        }

        private bool VideoExists(int id)
        {
            return (_context.Videos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
