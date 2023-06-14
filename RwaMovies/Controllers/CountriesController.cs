using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RwaMovies.DTOs;
using RwaMovies.Models;

namespace RwaMovies.Controllers
{
    public class CountriesController : Controller
    {
        private readonly RwaMoviesContext _context;
        private readonly IMapper _mapper;

        public CountriesController(RwaMoviesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int? pageNum)
        {
            pageNum ??= 1;
            var pageSize = 5;
            var countryCount = await _context.Countries.CountAsync();
            var countries = await _context.Countries
                .Skip((int)((pageNum - 1) * pageSize))
                .Take(pageSize)
                .ToListAsync();
            return View(new CountriesVM
            {
                Countries = _mapper.Map<List<CountryDTO>>(countries),
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
            return View(_mapper.Map<CountryDTO>(country));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CountryDTO countryDTO)
        {
            if (!ModelState.IsValid)
                return View(countryDTO);
            _context.Countries.Add(_mapper.Map<Country>(countryDTO));
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
                return NotFound();
            return View(_mapper.Map<CountryDTO>(country));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CountryDTO countryDTO)
        {
            if (id != countryDTO.Id)
                return NotFound();
            if (!ModelState.IsValid)
                return View(countryDTO);
            try
            {
                var country = _mapper.Map<Country>(countryDTO);
                _context.Entry(country).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                if (!CountryExists(id))
                    return NotFound();
                throw;
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var country = await _context.Countries.FirstOrDefaultAsync(m => m.Id == id);
            if (country == null)
                return NotFound();
            return View(_mapper.Map<CountryDTO>(country));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var country = await _context.Countries.FindAsync(id);
            if (country == null)
                return NotFound();
            try
            {
                _context.Countries.Remove(country);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqlException sqlEx && sqlEx.Message.Contains("FK_User_Country"))
                {
                    ModelState.AddModelError("", "Cannot delete country because it is used by one or more users.");
                    return View(_mapper.Map<CountryDTO>(country));
                }
                throw;
            }
        }

        private bool CountryExists(int id)
        {
            return (_context.Countries?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
