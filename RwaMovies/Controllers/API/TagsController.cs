using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RwaMovies.Exceptions;
using RwaMovies.Services;
using RwaMovies.Models.Shared;

namespace RwaMovies.Controllers.API
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]"), Area("API")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITagsService _tagsService;

        public TagsController(ITagsService tagsService)
        {
            _tagsService = tagsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDTO>>> GetTags()
        {
            return Ok(await _tagsService.GetTags());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TagDTO>> GetTag(int id)
        {
            try
            {
                return Ok(await _tagsService.GetTag(id));
            }
            catch
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTag(int id, TagDTO tagDTO)
        {
            if (!ModelState.IsValid || id != tagDTO.Id)
                return BadRequest();
            try
            {
                await _tagsService.PutTag(id, tagDTO);
                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> PostTag(TagDTO tagDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var tagId = await _tagsService.PostTag(tagDTO);
            return CreatedAtAction("GetTag", new { id = tagId });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            try
            {
                await _tagsService.DeleteTag(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                    return NotFound();
                if (ex.InnerException is SqlException sqlEx && sqlEx.Message.Contains("FK_Video_Tag"))
                    return Conflict();
                throw;
            }
        }
    }
}
