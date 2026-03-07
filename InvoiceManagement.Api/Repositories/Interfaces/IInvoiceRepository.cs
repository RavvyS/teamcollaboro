using InvoiceManagement.Api.Models;

namespace InvoiceManagement.Api.Repositories.Interfaces;

public interface IInvoiceRepository
{
    Task<int> CreateAsync(Invoice invoice);
    Task<IEnumerable<Invoice>> GetAllAsync();
    Task<Invoice?> GetByIdAsync(int invoiceId);
    Task<bool> UpdateAsync(Invoice invoice);
    Task<bool> DeleteAsync(int invoiceId);
}
