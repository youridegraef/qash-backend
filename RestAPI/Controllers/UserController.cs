using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;
using RestAPI.Models.RequestModels;

namespace RestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public UserController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hello World");
    }

    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginRequest loginRequest)
    {
        if (loginRequest == null)
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        string key = _configuration["Jwt:Key"];
        string issuer = _configuration["Jwt:Issuer"];
        AuthenticationDto dto = _userService.Authenticate(loginRequest.Email, loginRequest.Password, key, issuer);

        if (dto.User == null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        UserModel loggedInUser = new UserModel(dto.User.Id, dto.User.Name, dto.User.Email, dto.User.DateOfBirth);

        // Geef token én user terug
        return Ok(new
        {
            dto.Token,
            loggedInUser
        });
    }


    [HttpPost("Register")]
    public IActionResult Register([FromBody] RegisterRequest registerRequest)
    {
        if (registerRequest == null)
        {
            return BadRequest(new { messae = "Name, Email, Passowrd and DateOfBirth are required." });
        }

        User user = _userService.Register(registerRequest.Name, registerRequest.Email, registerRequest.Password,
            registerRequest.DateOfBirth);

        if (user == null)
        {
            return Conflict(new { message = "User registration failed. Email might already be in use." });
        }

        user = _userService.GetByEmail(user.Email);
        UserModel LoggedInUser = new UserModel(user.Id, user.Name, user.Email, user.DateOfBirth);

        return Ok(LoggedInUser);
    }
}