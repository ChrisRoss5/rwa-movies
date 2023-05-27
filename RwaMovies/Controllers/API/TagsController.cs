using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RwaMovies.DTOs;
using RwaMovies.Exceptions;
using RwaMovies.Models;
using RwaMovies.Services;

namespace RwaMovies.Controllers.API
{
    [Route("api/[controller]")]
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

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTag(int id, TagDTO tagDTO)
        {
            if (!ModelState.IsValid || id != tagDTO.Id)
                return BadRequest();
            try
            {
                await _tagsService.PutTag(id, tagDTO);
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
        public async Task<IActionResult> PostTag(TagDTO tagDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            try
            {
                var tagId = await _tagsService.PostTag(tagDTO);
                return CreatedAtAction("GetTag", new { id = tagId });
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
            try
            {
                await _tagsService.DeleteTag(id);
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
