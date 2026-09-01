using JoinTogether.BLL.Interfaces;
using JoinTogether.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace JoinTogether.API.Controllers;

// AuthController handles user authentication, including registration and login.
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var (success, error, result) = await _authService.RegisterAsync(dto);
        if (!success)
            return BadRequest(new { message = error });

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var (success, error, result) = await _authService.LoginAsync(dto);
        if (!success)
            return Unauthorized(new { message = error });

        return Ok(result);
    }
}