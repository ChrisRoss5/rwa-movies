using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RwaMovies.DTOs;
using RwaMovies.Exceptions;
using RwaMovies.Models;
using RwaMovies.Services;

namespace RwaMovies.Controllers
{
    public class TagsController : Controller
    {
        private readonly RwaMoviesContext _context;
        private readonly ITagsService _tagsService;

        public TagsController(RwaMoviesContext context, ITagsService tagsService)
        {
            _context = context;
            _tagsService = tagsService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _tagsService.GetTags());
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                return View(await _tagsService.GetTag(id));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                    return NotFound();
                throw;
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TagDTO tagDTO)
        {
            if (!ModelState.IsValid)
                return View(tagDTO);
            await _tagsService.PostTag(tagDTO);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                return View(await _tagsService.GetTag(id));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                    return NotFound();
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TagDTO tagDTO)
        {
            if (id != tagDTO.Id)
                return NotFound();
            if (!ModelState.IsValid)
                return View(tagDTO);
            try
            {
                await _tagsService.PutTag(id, tagDTO);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                    return NotFound();
                throw;
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                return View(await _tagsService.GetTag(id));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                    return NotFound();
                throw;
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _tagsService.DeleteTag(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                //Todo
                if (ex is NotFoundException)
                    return NotFound();
                throw;
            }
        }
    }
}
