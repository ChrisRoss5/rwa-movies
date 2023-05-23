using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RwaMovies.DTOs;
using RwaMovies.Models;

namespace RwaMovies.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly RwaMoviesContext _context;
        private readonly IMapper _mapper;

        public GenresController(RwaMoviesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenreDTO>>> GetGenres()
        {
            var genres = await _context.Genres.ToListAsync();
            return _mapper.Map<List<GenreDTO>>(genres);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GenreDTO>> GetGenre(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            return genre != null ? _mapper.Map<GenreDTO>(genre) : NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutGenre(int id, GenreDTO genreDTO)
        {
            if (!ModelState.IsValid || id != genreDTO.Id)
                return BadRequest();
            try
            {
                var genre = _mapper.Map<Genre>(genreDTO);
                _context.Entry(genre).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!GenreExists(id))
                    return NotFound();
                if (ex is DbUpdateException)
                    return StatusCode(StatusCodes.Status500InternalServerError, "DbUpdateException!");
                return StatusCode(StatusCodes.Status500InternalServerError, "Unknown error!");
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> PostGenre(GenreDTO genreDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                var genre = _mapper.Map<Genre>(genreDTO);
                _context.Genres.Add(genre);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetGenre", new { id = genre.Id });
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
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
                return NotFound();
            try
            {
                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Unable to delete genre with id={id}. Videos are using this genre.");
            }
        }

        private bool GenreExists(int id)
        {
            return (_context.Genres?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
