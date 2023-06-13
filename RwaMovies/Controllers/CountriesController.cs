using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RwaMovies.DTOs;
using RwaMovies.Models;
using RwaMovies.Services;

namespace RwaMovies.Controllers
{
    public class CountriesController : Controller
    {
        private readonly RwaMoviesContext _context;

        public CountriesController(RwaMoviesContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? pageNum)
        {
            pageNum ??= 1;
            var pageSize = 2;
            var countryCount = await _context.Countries.CountAsync();
            var countries = await _context.Countries
                .Skip((int)((pageNum - 1) * pageSize))
                .Take(pageSize)
                .ToListAsync();
            return View(new CountriesVM
            {
                Countries = countries,
                PageSize = pageSize,
                PageNum = pageNum.GetValueOrDefault(),
                PageCount = (int)Math.Ceiling((double)countryCount / pageSize)
            });
        }

        public async Task<IActionResult> Details(int id)
        {
            var country = await _context.Countries.FirstOrDefaultAsync(m => m.Id == id);
            if (country == null)
                return NotFound();
            return View(country);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Name")] Country country)
        {
            if (ModelState.IsValid)
            {
                _context.Add(country);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
                return NotFound();
            return View(country);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Name")] Country country)
        {
            if (id != country.Id)
                return NotFound();
            if (!ModelState.IsValid)
                return View(country);
            try
            {
                _context.Update(country);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(country.Id))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var country = await _context.Countries.FirstOrDefaultAsync(m => m.Id == id);
            if (country == null)
                return NotFound();
            return View(country);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country != null)
                _context.Countries.Remove(country);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CountryExists(int id)
        {
            return (_context.Countries?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
