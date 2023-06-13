using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RwaMovies.Services;
using RwaMovies.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using RwaMovies.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RwaMovies.Controllers
{
    public class AuthController : Controller
    {
        private readonly RwaMoviesContext _context;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(RwaMoviesContext context, IAuthService authService, IMapper mapper)
        {
            _context = context;
            _authService = authService;
            _mapper = mapper;
        }

        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {            
            if (IsAuthenticated())
                return Redirect("~/");
            ViewBag.RegisteredMessage = TempData["RegisteredMessage"];
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AuthRequest authRequest, string? returnUrl = null)
        {
            if (IsAuthenticated())
                return Redirect("~/");
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid)
                return View(authRequest);
            try
            {
                var user = await _authService.GetUser(authRequest);
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, _authService.GetRole(user))
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties());
                if (!string.IsNullOrEmpty(returnUrl))
                    return LocalRedirect(returnUrl);
                return Redirect("~/");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(authRequest);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Register()
        {
            if (IsAuthenticated())
                return Redirect("~/");
            await PopulateRegisterViewBag();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRequest userRequest)
        {
            if (IsAuthenticated())
                return Redirect("~/");
            if (!ModelState.IsValid)
            {
                await PopulateRegisterViewBag();
                return View(userRequest);
            }
            try
            {
                await _authService.Register(userRequest);
                TempData["RegisteredMessage"] = $"An email has been sent to {userRequest.Email}. Please confirm your email address to complete registration.";
                return RedirectToAction(nameof(Login));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                await PopulateRegisterViewBag();
                return View(userRequest);
            }
        }

        private async Task PopulateRegisterViewBag()
        {
            var countries = await _context.Countries.ToListAsync();
            ViewBag.CountryOfResidenceId = new SelectList(countries, "Id", "Name");
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string username, string b64SecToken)
        {
            if (IsAuthenticated())
                return Redirect("~/");
            try
            {
                await _authService.ConfirmEmail(username, b64SecToken);
                ViewBag.Success = true;
                ViewBag.Message = "Email confirmed successfully!";
            }
            catch (InvalidOperationException ex)
            {
                ViewBag.Success = false;
                ViewBag.Message = ex.Message;
            }
            return View();
        }

        public async Task<IActionResult> Profile()
        {
            var user = await _context.Users
                .Include(u => u.CountryOfResidence)
                .FirstOrDefaultAsync(u => u.Username == HttpContext.User.Identity!.Name);
            return View(_mapper.Map<UserResponse>(user));
        }

        public IActionResult ChangePassword()
        {
            ViewBag.IsSuccess = TempData["IsSuccess"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(NewPasswordRequest request)
        {
            ModelState.Remove("AuthRequest.Username");
            request.AuthRequest.Username = HttpContext.User.Identity?.Name!;
            if (!ModelState.IsValid)
                return View(request);
            try
            {
                await _authService.ChangePassword(request);
                TempData["IsSuccess"] = true;
                return RedirectToAction(nameof(ChangePassword));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("",
                    ex.Message == "Incorrect username or password" ? "Incorrect current password" : ex.Message);
                return View(request);
            }
        }

        private bool IsAuthenticated() => HttpContext.User.Identity?.IsAuthenticated ?? false;
    }
}
