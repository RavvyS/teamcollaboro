using InvoiceManagement.Api.Models;

namespace InvoiceManagement.Api.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
