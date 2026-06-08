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
        if (dto.Username == "admin" &&
            dto.Password == "admin123")
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