using InvoiceManagement.Api.DTOs.Auth;
using InvoiceManagement.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceManagement.Api.Controllers;

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
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var success = await _authService.RegisterAsync(request);
        if (!success)
        {
            return Conflict(new { message = "Email is already registered" });
        }

        return Created(string.Empty, new { message = "User registered successfully" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var response = await _authService.LoginAsync(request);
        if (response is null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        return Ok(response);
    }
}
