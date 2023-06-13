using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RwaMovies.DTOs;
using RwaMovies.Exceptions;
using RwaMovies.Models;
using System.Linq.Dynamic.Core;

namespace RwaMovies.Services
{
    public class SearchParams
    {
        public string? NameFilter { get; set; }
        public string? GenreFilter { get; set; }
        public string? OrderBy { get; set; }
        public string? OrderDirection { get; set; }
        public int? PageNum { get; set; }
        public int? PageSize { get; set; }
    }

    public class SearchResult
    {
        public List<VideoResponse> Videos { get; set; } = null!;
        public int FilteredVideoCount { get; set; }
        public int PageSize { get; set; }
        public int PageNum { get; set; }
        public int PageCount { get; set; }
    }

    public interface IVideosService
    {
        Task<IEnumerable<VideoResponse>> GetVideos();
        Task<VideoResponse> GetVideo(int id);
        Task<SearchResult> Search(SearchParams sp);
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
                .Include(v => v.VideoTags).ThenInclude(vt => vt.Tag)
                .ToListAsync();
            return _mapper.Map<List<VideoResponse>>(videos);
        }

        public async Task<VideoResponse> GetVideo(int id)
        {
            var video = await _context.Videos
                .Where(v => v.Id == id)
                .Include(v => v.Genre)
                .Include(v => v.VideoTags).ThenInclude(vt => vt.Tag)
                .FirstOrDefaultAsync();
            if (video is null)
                throw new NotFoundException();
            return _mapper.Map<VideoResponse>(video);
        }

        public async Task<SearchResult> Search(SearchParams sp)
        {
            sp.OrderBy ??= "Id";
            sp.OrderDirection ??= "asc";
            sp.PageNum ??= 1;
            sp.PageSize ??= 6;
            if (!new[] { "Id", "Name", "TotalSeconds" }.Contains(sp.OrderBy) ||
                !new[] { "asc", "desc" }.Contains(sp.OrderDirection) || sp.PageNum < 1 || sp.PageSize < 1)
                throw new BadRequestException();
            var videosFiltered = _context.Videos
                .Where(v => (string.IsNullOrEmpty(sp.NameFilter) || v.Name.Contains(sp.NameFilter))
                    && (string.IsNullOrEmpty(sp.GenreFilter) || v.Genre.Name == sp.GenreFilter));
            var filteredVideoCount = await videosFiltered.CountAsync();
            var videos = await videosFiltered
                .OrderBy($"{sp.OrderBy} {sp.OrderDirection}")
                .Skip((int)((sp.PageNum - 1) * sp.PageSize))
                .Take((int)sp.PageSize)
                .Include(v => v.Genre)
                .Include(v => v.VideoTags).ThenInclude(vt => vt.Tag)
                .ToListAsync();
            return new SearchResult
            {
                Videos = _mapper.Map<List<VideoResponse>>(videos).ToList(),
                FilteredVideoCount = filteredVideoCount,
                PageSize = (int)sp.PageSize,
                PageNum = (int)sp.PageNum,
                PageCount = (int)Math.Ceiling(filteredVideoCount / (double)sp.PageSize)
            };
        }

        public async Task PutVideo(int id, VideoRequest videoRequest)
        {
            try
            {
                var video = _mapper.Map<Video>(videoRequest);
                var existingVideoTags = await _context.VideoTags.Where(vt => vt.VideoId == id).ToListAsync();
                _context.VideoTags.RemoveRange(existingVideoTags);
                if (videoRequest.TagIds != null)
                {
                    var newVideoTags = videoRequest.TagIds.Select(x => new VideoTag { VideoId = id, TagId = x });
                    _context.VideoTags.AddRange(newVideoTags);
                }
                var state = _context.Entry(video).State;
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
            if (videoRequest.TagIds != null)
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
