using AssignmentApi.DTOs;
using AssignmentApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;

    public AuthController(JwtService jwtService)
    {
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public IActionResult Login(LoginDto dto)
    {
        var username = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(dto.Username));
        var password = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(dto.Password));
        if (username == "admin" && password == "admin123")
        {
            var token = _jwtService.GenerateToken(dto.Username);

            return Ok(new
            {
                token
            });
        }

        return Unauthorized();
    }
}