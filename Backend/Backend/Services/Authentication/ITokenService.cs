using Microsoft.AspNetCore.Identity;

namespace Backend.Services.Authentication;

public interface ITokenService
{
    string CreateToken(IdentityUser user, string role);
}