using Application.Exceptions;
using Google.Protobuf.WellKnownTypes;

namespace RestAPI.ResponseModels;

public class RegisterResponse(int id, string name, string email)
{
    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string Email { get; set; } = email;
}