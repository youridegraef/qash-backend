using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;

namespace RestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService, IConfiguration configuration) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hello World");
    }

    [HttpPost("Login")]
    public IActionResult Login([FromBody] Requests.Login req)
    {
        if (req == null!)
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        string key = configuration["Jwt:Key"]!;
        string issuer = configuration["Jwt:Issuer"]!;
        AuthenticationDto dto = userService.Authenticate(req.Email, req.Password, key, issuer);

        if (dto.User == null!)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var loggedInUser = new { dto.User.Id, dto.User.Name, dto.User.Email };

        return Ok(new
        {
            dto.Token, loggedInUser
        });
    }


    [HttpPost("Register")]
    public IActionResult Register([FromBody] Requests.Register req)
    {
        if (req.Name == null! || req.Email == null! || req.Password == null! || req.DateOfBirth == null)
        {
            return BadRequest(new { messae = "Name, Email, uPassword and DateOfBirth are required." });
        }

        User user = userService.Register(req.Name, req.Email, req.Password, req.DateOfBirth);

        if (user == null!)
        {
            return Conflict(new { message = "User registration failed. Email might already be in use." });
        }

        return Ok(new { user.Name, user.Email, user.DateOfBirth });
    }
}