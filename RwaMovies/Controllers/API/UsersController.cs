using AutoMapper;
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
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<User>> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                return Ok(await _usersService.Register(request));
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException)
                    return BadRequest(ex.Message);
                if (ex is DbUpdateException)
                    return StatusCode(StatusCodes.Status500InternalServerError, "DbUpdateException!");
                throw;
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> ValidateEmail([FromBody] ValidateEmailRequest request)
        {
            try
            {
                await _usersService.ValidateEmail(request);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<Tokens>> JwtTokens([FromBody] LoginRequest request)
        {
            try
            {
                return Ok(await _usersService.JwtTokens(request));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                await _usersService.ChangePassword(request);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
