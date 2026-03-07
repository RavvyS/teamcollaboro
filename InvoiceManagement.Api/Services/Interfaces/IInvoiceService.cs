using InvoiceManagement.Api.DTOs.Invoice;

namespace InvoiceManagement.Api.Services.Interfaces;

public interface IInvoiceService
{
    Task<InvoiceResponse> CreateAsync(CreateInvoiceRequest request);
    Task<IEnumerable<InvoiceResponse>> GetAllAsync();
    Task<InvoiceResponse?> GetByIdAsync(int invoiceId);
    Task<InvoiceResponse?> UpdateAsync(int invoiceId, UpdateInvoiceRequest request);
    Task<bool> DeleteAsync(int invoiceId);
}
