using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RwaMovies.SharedModels;
using RwaMovies.Exceptions;
using RwaMovies.Models;
using RwaMovies.Services;
using RwaMovies.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace RwaMovies.Controllers.Views
{
    public class GenresController : Controller
    {
        private readonly RwaMoviesContext _context;
        private readonly IGenresService _genresService;

        public GenresController(RwaMoviesContext context, IGenresService genresService)
        {
            _context = context;
            _genresService = genresService;
        }

        public async Task<IActionResult> Index()
        {
            var genres = await _genresService.GetGenres();
            if (Request.IsAjaxRequest())
                return PartialView(genres);
            return View(genres);
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var genre = await _genresService.GetGenre(id);
                if (Request.IsAjaxRequest())
                    return PartialView(genre);
                return View(genre);
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                    return NotFound();
                throw;
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            if (Request.IsAjaxRequest())
                return PartialView();
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GenreDTO genreDTO)
        {
            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                    return PartialView(genreDTO);
                return View(genreDTO);
            }
            await _genresService.PostGenre(genreDTO);
            if (Request.IsAjaxRequest())
                return Ok("Success");
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var genre = await _genresService.GetGenre(id);
                if (Request.IsAjaxRequest())
                    return PartialView(genre);
                return View(genre);
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                    return NotFound();
                throw;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GenreDTO genreDTO)
        {
            if (id != genreDTO.Id)
                return NotFound();
            if (!ModelState.IsValid)
            {
                if (Request.IsAjaxRequest())
                    return PartialView(genreDTO);
                return View(genreDTO);
            }
            try
            {
                await _genresService.PutGenre(id, genreDTO);
                if (Request.IsAjaxRequest())
                    return Ok("Success");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                    return NotFound();
                throw;
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var genre = await _genresService.GetGenre(id);
                if (Request.IsAjaxRequest())
                    return PartialView(genre);
                return View(genre);
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                    return NotFound();
                throw;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _genresService.DeleteGenre(id);
                if (Request.IsAjaxRequest())
                    return Ok("Success");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                    return NotFound();
                if (ex.InnerException is SqlException sqlEx && sqlEx.Message.Contains("FK_Video_Genre"))
                {
                    ModelState.AddModelError("", "Cannot delete genre because it is used in one or more videos.");
                    var genre = await _genresService.GetGenre(id);
                    if (Request.IsAjaxRequest())
                        return PartialView(genre);
                    return View(genre);
                }
                throw;
            }
        }
    }
}
