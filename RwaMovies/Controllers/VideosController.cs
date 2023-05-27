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
    public class VideosController : Controller
    {
        private readonly IVideosService _videosService;
        private readonly IGenresService _genresService;

        public VideosController(IVideosService videosService, IGenresService genresService)
        {
            _videosService = videosService;
            _genresService = genresService;
        }

        public async Task<IActionResult> Index(
            string? nameFilter, string? genreFilter, string? orderBy, string? orderDirection, int? page, int? size)
        {
            try
            {
                var videos = await _videosService.Search(nameFilter, orderBy, orderDirection, page, size);
                var genres = await _genresService.GetGenres();
                if (!string.IsNullOrEmpty(genreFilter))
                    videos = videos.Where(v => v.Genre.Name == genreFilter);
                var VideoGenreVM = new VideoGenreViewModel
                {
                    Videos = videos.ToList(),
                    Genres = new SelectList(genres.Select(g => g.Name)),
                };
                ViewData["NameFilter"] = nameFilter;
                ViewData["GenreFilter"] = genreFilter;
                return View(VideoGenreVM);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unknown error!");
            }
        }


        public async Task<IActionResult> Details(int id)
        {
            try
            {
                return View(await _videosService.GetVideo(id));
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        public IActionResult Create()
        {
            /*ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Id");
            ViewData["ImageId"] = new SelectList(_context.Images, "Id", "Id");*/
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CreatedAt,Name,Description,GenreId,TotalSeconds,StreamingUrl,ImageId")] Video video)
        {
            /*if (ModelState.IsValid)
            {
                _context.Add(video);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Id", video.GenreId);
            ViewData["ImageId"] = new SelectList(_context.Images, "Id", "Id", video.ImageId);*/
            return View(video);
        }

        public async Task<IActionResult> Edit(int id)
        {
            return NotFound();

            /*var video = await _context.Videos.FindAsync(id);
            if (video == null)
            {
                return NotFound();
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Id", video.GenreId);
            ViewData["ImageId"] = new SelectList(_context.Images, "Id", "Id", video.ImageId);
            return View(video);*/
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CreatedAt,Name,Description,GenreId,TotalSeconds,StreamingUrl,ImageId")] Video video)
        {
            if (id != video.Id)
            {
                return NotFound();
            }
            return NotFound();

            /*if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(video);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VideoExists(video.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Id", video.GenreId);
            ViewData["ImageId"] = new SelectList(_context.Images, "Id", "Id", video.ImageId);
            return View(video);*/
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                return View(await _videosService.GetVideo(id));
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _videosService.DeleteVideo(id);
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }
    }
}
