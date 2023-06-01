using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RwaMovies.DTOs;
using RwaMovies.Exceptions;
using RwaMovies.Services;

namespace RwaMovies.Controllers.API
{
	[Route("api/[controller]")]
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

        [HttpPut("{id}")]
        public async Task<IActionResult> PutGenre(int id, GenreDTO genreDTO)
        {
            if (!ModelState.IsValid || id != genreDTO.Id)
                return BadRequest();
            try
            {
                await _genresService.PutGenre(id, genreDTO);
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
        public async Task<IActionResult> PostGenre(GenreDTO genreDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                var genreId = await _genresService.PostGenre(genreDTO);
                return CreatedAtAction("GetGenre", new { id = genreId });
            }
            catch (Exception ex)
            {
                if (ex is DbUpdateException)
                    return StatusCode(StatusCodes.Status500InternalServerError, "DbUpdateException!");
                return StatusCode(StatusCodes.Status500InternalServerError, "Unknown error!");
            }
        }

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
                if (ex is DbUpdateException)
                    return StatusCode(StatusCodes.Status500InternalServerError, "DbUpdateException!");
                return StatusCode(StatusCodes.Status500InternalServerError, "Unknown error!");
            }
        }
    }
}
