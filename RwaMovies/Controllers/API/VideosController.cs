using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RwaMovies.DTOs;
using RwaMovies.Models;

namespace RwaMovies.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly RwaMoviesContext _context;
        private readonly IMapper _mapper;

        public VideosController(RwaMoviesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VideoResponse>>> GetVideos()
        {
            var videos = await _context.Videos
                .Include(v => v.Genre)
                .Include(v => v.Image)
                .Include(v => v.VideoTags).ThenInclude(vt => vt.Tag)
                .ToListAsync();
            return _mapper.Map<List<VideoResponse>>(videos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VideoResponse>> GetVideo(int id)
        {
            var video = await _context.Videos
                .Include(v => v.Genre)
                .Include(v => v.Image)
                .Include(v => v.VideoTags).ThenInclude(vt => vt.Tag)
                .FirstOrDefaultAsync(v => v.Id == id);
            return video != null ? _mapper.Map<VideoResponse>(video) : NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutVideo(int id, VideoRequest videoRequest)
        {
            if (!ModelState.IsValid || id != videoRequest.Id)
                return BadRequest();
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
            catch (Exception ex)
            {
                if (!VideoExists(id))
                    return NotFound();
                if (ex is DbUpdateException)
                    return StatusCode(StatusCodes.Status500InternalServerError, "DbUpdateException!");
                return StatusCode(StatusCodes.Status500InternalServerError, "Unknown error!");
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> PostVideo(VideoRequest videoRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                var video = _mapper.Map<Video>(videoRequest);
                video.VideoTags = videoRequest.TagIds.Select(x => new VideoTag { TagId = x }).ToList();
                _context.Videos.Add(video);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetVideo", new { id = video.Id });
            }
            catch (Exception ex)
            {
                if (ex is DbUpdateException)
                    return StatusCode(StatusCodes.Status500InternalServerError, "DbUpdateException!");
                return StatusCode(StatusCodes.Status500InternalServerError, "Unknown error!");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            var video = await _context.Videos.Include(v => v.VideoTags).FirstOrDefaultAsync(v => v.Id == id);
            if (video == null)
                return NotFound();
            _context.Videos.Remove(video);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool VideoExists(int id)
        {
            return (_context.Videos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
