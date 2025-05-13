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

        if (dto.UserAuthenticate == null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        UserModel loggedInUser = new UserModel(dto.UserAuthenticate.Id, dto.UserAuthenticate.Name, dto.UserAuthenticate.Email, dto.UserAuthenticate.DateOfBirth);

        // Geef token én userAuthenticate terug
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

        UserAuthenticate userAuthenticate = _userService.Register(registerRequest.Name, registerRequest.Email, registerRequest.Password,
            registerRequest.DateOfBirth);

        if (userAuthenticate == null)
        {
            return Conflict(new { message = "UserAuthenticate registration failed. Email might already be in use." });
        }

        userAuthenticate = _userService.GetByEmail(userAuthenticate.Email);
        UserModel LoggedInUser = new UserModel(userAuthenticate.Id, userAuthenticate.Name, userAuthenticate.Email, userAuthenticate.DateOfBirth);

        return Ok(LoggedInUser);
    }
}