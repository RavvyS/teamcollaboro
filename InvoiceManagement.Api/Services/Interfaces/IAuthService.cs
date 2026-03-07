using InvoiceManagement.Api.DTOs.Auth;

namespace InvoiceManagement.Api.Services.Interfaces;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterUserRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);
}
