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
    public class GenresController : ControllerBase
    {
        private readonly IGenresService _genresService;

        public GenresController(IGenresService genresService)
        {
            _genresService = genresService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenreDTO>>> GetGenres()
        {
            return Ok(await _genresService.GetGenres());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GenreDTO>> GetGenre(int id)
        {
            try
            {
                return Ok(await _genresService.GetGenre(id));
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGenre(int id, GenreDTO genreDTO)
        {
            if (!ModelState.IsValid || id != genreDTO.Id)
                return BadRequest(ModelState);
            try
            {
                await _genresService.PutGenre(id, genreDTO);
                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> PostGenre(GenreDTO genreDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var genreId = await _genresService.PostGenre(genreDTO);
            return CreatedAtAction("GetGenre", new { id = genreId });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            try
            {
                await _genresService.DeleteGenre(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                    return NotFound();
                if (ex.InnerException is SqlException sqlEx && sqlEx.Message.Contains("FK_Video_Genre"))
                    return Conflict("Cannot delete genre because it is used in a video.");
                throw;
            }
        }
    }
}
