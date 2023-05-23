using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RwaMovies.DTOs;
using RwaMovies.Models;

namespace RwaMovies.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly RwaMoviesContext _context;
        private readonly IMapper _mapper;

        public TagsController(RwaMoviesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDTO>>> GetTags()
        {
            var tags = await _context.Tags.ToListAsync();
            return _mapper.Map<List<TagDTO>>(tags);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TagDTO>> GetTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            return tag != null ? _mapper.Map<TagDTO>(tag) : NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTag(int id, TagDTO tagDTO)
        {
            if (!ModelState.IsValid || id != tagDTO.Id)
                return BadRequest();
            try
            {
                var tag = _mapper.Map<Tag>(tagDTO);
                _context.Entry(tag).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (!TagExists(id))
                    return NotFound();
                if (ex is DbUpdateException)
                    return StatusCode(StatusCodes.Status500InternalServerError, "DbUpdateException!");
                return StatusCode(StatusCodes.Status500InternalServerError, "Unknown error!");
            }
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> PostTag(TagDTO tagDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                var tag = _mapper.Map<Tag>(tagDTO);
                _context.Tags.Add(tag);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetTag", new { id = tag.Id });
            }
            catch (Exception ex)
            {
                if (ex is DbUpdateException)
                    return StatusCode(StatusCodes.Status500InternalServerError, "DbUpdateException!");
                return StatusCode(StatusCodes.Status500InternalServerError, "Unknown error!");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var tag = await _context.Tags.Include(t => t.VideoTags).FirstOrDefaultAsync(t => t.Id == id);
            if (tag == null)
                return NotFound();
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool TagExists(int id)
        {
            return (_context.Tags?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
