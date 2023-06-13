using AutoMapper;
using Azure.Identity;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RwaMovies.DTOs.Auth;
using RwaMovies.Exceptions;
using RwaMovies.Models;
using RwaMovies.Services;
using System.Numerics;
using System.Security.Cryptography;

namespace RwaMovies.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /*[HttpPost("[action]")]
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
                return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
            }
        }*/

        [HttpPost("[action]")]
        public async Task<ActionResult<string>> GetJwtToken(AuthRequest request)
        {
            try
            {
                return Ok(await _authService.GetJwtToken(request));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
            }
        }

        /*[HttpPost("[action]")] todo
        public async Task<ActionResult> ChangePassword(NewPasswordRequest request)
        {
            try
            {
                await _authService.ChangePassword(request);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
            }
        }*/
    }
}
