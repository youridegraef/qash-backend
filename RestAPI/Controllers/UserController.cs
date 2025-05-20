using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RestAPI.RequestModels;
using RestAPI.ResponseModels;

namespace RestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService, IConfiguration configuration) : ControllerBase
{
    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        if (req == null!)
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        var key = configuration["Jwt:Key"]!;
        var issuer = configuration["Jwt:Issuer"]!;
        var dto = userService.Authenticate(req.Email, req.Password, key, issuer);

        if (dto.User == null!)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        return Ok(new LoginResponse(dto.Token, dto.User));
    }


    [HttpPost("Register")]
    public IActionResult Register([FromBody] RegisterRequest req)
    {
        if (req.Name == null! || req.Email == null! || req.Password == null!)
        {
            return BadRequest(new { messae = "Name, Email, Password and DateOfBirth are required." });
        }

        var user = userService.Register(req.Name, req.Email, req.Password);

        if (user == null!)
        {
            return Conflict(new { message = "User registration failed. Email might already be in use." });
        }

        return Ok(new RegisterResponse(user.Id, user.Name, user.Email));
    }
}