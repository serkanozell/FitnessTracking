using FitnessTracking.Mvc.Models;

namespace FitnessTracking.Mvc.Services.Auth;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequest request);
    Task<bool> RegisterAsync(RegisterRequest request);
}
