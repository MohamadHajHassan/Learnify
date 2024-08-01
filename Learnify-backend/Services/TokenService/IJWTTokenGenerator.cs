using Learnify_backend.Entities;

namespace Learnify_backend.Services.TokenService
{
    public interface IJWTTokenGenerator
    {
        string GenerateJwtToken(User user);
    }
}
