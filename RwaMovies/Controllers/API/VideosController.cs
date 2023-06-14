using Microsoft.AspNetCore.Mvc;
using RwaMovies.Services;
using RwaMovies.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using RwaMovies.Models.Shared;

namespace RwaMovies.Controllers.API
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]"), Area("API")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly IVideosService _videosService;

        public VideosController(IVideosService videosService)
        {
            _videosService = videosService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VideoResponse>>> GetVideos()
        {
            return Ok(await _videosService.GetVideos());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VideoResponse>> GetVideo(int id)
        {
            try
            {
                return Ok(await _videosService.GetVideo(id));
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<SearchResult>> Search(string? nameFilter, string? orderBy, string? orderDirection, int? pageNum, int? pageSize)
        {
            try
            {
                return Ok(await _videosService.Search(new SearchParams
                {
                    NameFilter = nameFilter,
                    OrderBy = orderBy,
                    OrderDirection = orderDirection,
                    PageNum = pageNum,
                    PageSize = pageSize
                }));
            }
            catch (BadRequestException)
            {
                return BadRequest();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVideo(int id, VideoRequest videoRequest)
        {
            if (!ModelState.IsValid || id != videoRequest.Id)
                return BadRequest();
            try
            {
                await _videosService.PutVideo(id, videoRequest);
                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> PostVideo(VideoRequest videoRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var videoId = await _videosService.PostVideo(videoRequest);
            return CreatedAtAction("GetVideo", new { id = videoId });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            try
            {
                await _videosService.DeleteVideo(id);
                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }
    }
}
