using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Models.RequestModels;
using RestAPI.Models.ResponseModels;

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
    public IActionResult Login([FromBody] LoginRequest loginRequest)
    {
        if (loginRequest == null!)
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        string key = configuration["Jwt:Key"]!;
        string issuer = configuration["Jwt:Issuer"]!;
        AuthenticationDto dto = userService.Authenticate(loginRequest.Email, loginRequest.Password, key, issuer);

        if (dto.User == null!)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        UserResponseModel loggedInUser = new UserResponseModel(dto.User.Id, dto.User.Name, dto.User.Email);

        return Ok(new
        {
            dto.Token,
            loggedInUser
        });
    }


    [HttpPost("Register")]
    public IActionResult Register([FromBody] RegisterRequest registerRequest)
    {
        if (registerRequest == null!)
        {
            return BadRequest(new { messae = "Name, Email, Passowrd and DateOfBirth are required." });
        }

        User user = userService.Register(registerRequest.Name, registerRequest.Email, registerRequest.Password,
            registerRequest.DateOfBirth);

        if (user == null!)
        {
            return Conflict(new { message = "User registration failed. Email might already be in use." });
        }

        user = userService.GetByEmail(user.Email);
        UserResponseModel loggedInUser = new UserResponseModel(user.Id, user.Name, user.Email);

        return Ok(loggedInUser);
    }
}