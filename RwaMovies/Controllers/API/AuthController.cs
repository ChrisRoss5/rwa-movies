using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RwaMovies.Services;
using RwaMovies.Models.DAL;
using RwaMovies.Models.Shared.Auth;

namespace RwaMovies.Controllers.API
{
    [AllowAnonymous]
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
        public async Task<ActionResult<User>> Register(UserRequest userRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
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
        public async Task<ActionResult> ChangePassword(NewPasswordRequest newPasswordRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await _authService.ChangePassword(newPasswordRequest);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
            }
        }
    }
}
