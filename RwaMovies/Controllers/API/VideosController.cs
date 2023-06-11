using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RwaMovies.DTOs;
using RwaMovies.Services;
using RwaMovies.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace RwaMovies.Controllers.API
{
    [Authorize]
    [Route("api/[controller]")]
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
			catch
			{
				return NotFound();
			}
		}

		[HttpGet("[action]")]
		public async Task<ActionResult<SearchResult>> Search(
			string? nameFilter, string? orderBy, string? orderDirection, int? pageNum, int? pageSize)
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
			catch (Exception ex)
			{
				if (ex is BadRequestException)
					return BadRequest();
				return StatusCode(StatusCodes.Status500InternalServerError, "Unknown error!");
			}
		}


		[HttpPut("{id}")]
		public async Task<IActionResult> PutVideo(int id, VideoRequest videoRequest)
		{
			if (!ModelState.IsValid || id != videoRequest.Id)
				return BadRequest();
			try
			{
				await _videosService.PutVideo(id, videoRequest);
				return NoContent();
			}
			catch (Exception ex)
			{
				if (ex is NotFoundException)
					return NotFound();
				if (ex is DbUpdateException)
					return StatusCode(StatusCodes.Status500InternalServerError, "DbUpdateException!");
				return StatusCode(StatusCodes.Status500InternalServerError, "Unknown error!");
			}
		}

		[HttpPost]
		public async Task<IActionResult> PostVideo(VideoRequest videoRequest)
		{
			if (!ModelState.IsValid)
				return BadRequest();
			try
			{
				var videoId = await _videosService.PostVideo(videoRequest);
				return CreatedAtAction("GetVideo", new { id = videoId });
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
			try
			{
				await _videosService.DeleteVideo(id);
				return NoContent();
			}
			catch (Exception ex)
			{
				if (ex is NotFoundException)
					return NotFound();
				if (ex is DbUpdateException)
					return StatusCode(StatusCodes.Status500InternalServerError, "DbUpdateException!");
				return StatusCode(StatusCodes.Status500InternalServerError, "Unknown error!");
			}
		}
	}
}
