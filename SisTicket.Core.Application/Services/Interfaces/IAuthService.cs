using SisTicket.Core.Application.DTOs.Auth;

namespace SisTicket.Core.Application.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<bool> ValidateTokenAsync(string token);
}
