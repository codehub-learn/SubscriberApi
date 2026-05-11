using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SubscriberApi.Auth;
using SubscriberApi.Dtos;

namespace SubscriberApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly JwtService _jwtService;
    public AuthController(JwtService jwtService) { _jwtService = jwtService; }


    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        // Fake validation
        if (request.Username == "admin" && request.Password == "1234")
        {
            var token = _jwtService.GenerateToken(request.Username, "Admin");
            return Ok(new { token });
        }
        return Unauthorized();
    }


    [HttpGet("secureMessage")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetSecureMessage()
    {
        return Ok("This is a secure message for authenticated users.");
    }



}





