using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RwaMovies.Services;
using RwaMovies.Models.Shared.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace RwaMovies.Controllers.API
{
    [AllowAnonymous]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]"), Area("API")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<string>> Register(UserRequest userRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                if (!User.IsInRole("Admin") && userRequest.IsConfirmed)
                    return StatusCode(StatusCodes.Status403Forbidden, "Only admins can create confirmed users.");
                await _authService.Register(userRequest);
                return Ok($"An email has been sent to {userRequest.Email}. Please confirm your email address to complete registration.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<string>> GetJwtToken(AuthRequest authRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                return Ok(await _authService.GetJwtToken(authRequest));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ChangePassword(NewPasswordRequest newPasswordRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await _authService.ChangePassword(newPasswordRequest);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
            }
        }
    }
}
