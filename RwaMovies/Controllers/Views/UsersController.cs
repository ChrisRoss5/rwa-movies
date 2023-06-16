using System.Data;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RwaMovies.Services;
using RwaMovies.Models.DAL;
using RwaMovies.Models.Views;
using RwaMovies.Models.Shared.Auth;

namespace RwaMovies.Controllers.Views
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly RwaMoviesContext _context;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public UsersController(RwaMoviesContext context, IMapper mapper, IAuthService authService)
        {
            _context = context;
            _mapper = mapper;
            _authService = authService;
        }

        public async Task<IActionResult> Index(string? firstNameFilter, string? lastNameFilter, string? usernameFilter, string? countryOfResidenceFilter)
        {
            var users = await _context.Users
                .Include(u => u.CountryOfResidence)
                .Where(u => (string.IsNullOrEmpty(firstNameFilter) || u.FirstName.Contains(firstNameFilter))
                    && (string.IsNullOrEmpty(lastNameFilter) || u.LastName.Contains(lastNameFilter))
                    && (string.IsNullOrEmpty(usernameFilter) || u.Username.Contains(usernameFilter))
                    && (string.IsNullOrEmpty(countryOfResidenceFilter) || u.CountryOfResidence.Name == countryOfResidenceFilter))
                .ToListAsync();
            var countries = await _context.Countries.ToListAsync();
            return View(new UsersVM
            {
                Users = _mapper.Map<List<UserResponse>>(users),
                Countries = new SelectList(countries.Select(c => c.Name)),
                FirstNameFilter = firstNameFilter,
                LastNameFilter = lastNameFilter,
                UsernameFilter = usernameFilter,
                CountryOfResidenceFilter = countryOfResidenceFilter,
            });
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users
                .Include(u => u.CountryOfResidence)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound();
            return View(_mapper.Map<UserResponse>(user));
        }

        public async Task<IActionResult> Create()
        {
            await PopulateUsersViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserRequestAdminVM userRequestAdmin)
        {
            if (!ModelState.IsValid)
            {
                await PopulateUsersViewBag();
                return View(userRequestAdmin);
            }
            try
            {
                await _authService.Register(userRequestAdmin.UserRequest, userRequestAdmin.IsConfirmed);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                await PopulateUsersViewBag();
                return View(userRequestAdmin);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();
            await PopulateUsersViewBag();
            return View(new UserRequestAdminVM
            {
                UserRequest = _mapper.Map<UserRequest>(user),
                IsConfirmed = user.IsConfirmed
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserRequestAdminVM userRequestAdmin)
        {
            var userRequest = userRequestAdmin.UserRequest;
            if (id != userRequest.Id)
                return NotFound();
            var passwordsMatch = userRequest.Password1 == userRequest.Password2;
            if (!ModelState.IsValid || !passwordsMatch)
            {
                if (!passwordsMatch)
                    ModelState.AddModelError("", "Passwords do not match.");
                await PopulateUsersViewBag();
                return View(userRequestAdmin);
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();
            _mapper.Map(userRequest, user);
            user.IsConfirmed = userRequestAdmin.IsConfirmed;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users
                .Include(u => u.CountryOfResidence)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
                return NotFound();
            return View(_mapper.Map<UserResponse>(user));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, bool? toggle)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();
            if (toggle == null || !toggle.Value)
                _context.Users.Remove(user);
            else
                user.DeletedAt = user.DeletedAt == null ? DateTime.Now : null;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Toggle")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleConfirmed(int id)
        {
            return await DeleteConfirmed(id, true);
        }

        private async Task PopulateUsersViewBag()
        {
            var countries = await _context.Countries.ToListAsync();
            ViewBag.CountryOfResidenceId = new SelectList(countries, "Id", "Name");
        }
    }
}
