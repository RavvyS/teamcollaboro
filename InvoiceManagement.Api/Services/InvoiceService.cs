using InvoiceManagement.Api.DTOs.Invoice;
using InvoiceManagement.Api.Models;
using InvoiceManagement.Api.Repositories.Interfaces;
using InvoiceManagement.Api.Services.Interfaces;

namespace InvoiceManagement.Api.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ILogger<InvoiceService> _logger;

    public InvoiceService(IInvoiceRepository invoiceRepository, ILogger<InvoiceService> logger)
    {
        _invoiceRepository = invoiceRepository;
        _logger = logger;
    }

    public async Task<InvoiceResponse> CreateAsync(CreateInvoiceRequest request)
    {
        var invoice = new Invoice
        {
            InvoiceNumber = request.InvoiceNumber,
            CustomerName = request.CustomerName,
            Amount = request.Amount,
            Status = request.Status,
            CreatedDate = DateTime.UtcNow
        };

        var id = await _invoiceRepository.CreateAsync(invoice);
        invoice.InvoiceId = id;

        if (string.Equals(invoice.Status, "Paid", StringComparison.OrdinalIgnoreCase))
        {
            _ = Task.Run(() => LogPaymentConfirmationAsync(invoice));
        }

        return ToResponse(invoice);
    }

    public async Task<IEnumerable<InvoiceResponse>> GetAllAsync()
    {
        var invoices = await _invoiceRepository.GetAllAsync();
        return invoices.Select(ToResponse);
    }

    public async Task<InvoiceResponse?> GetByIdAsync(int invoiceId)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
        return invoice is null ? null : ToResponse(invoice);
    }

    public async Task<InvoiceResponse?> UpdateAsync(int invoiceId, UpdateInvoiceRequest request)
    {
        var existing = await _invoiceRepository.GetByIdAsync(invoiceId);
        if (existing is null)
        {
            return null;
        }

        var wasPaid = string.Equals(existing.Status, "Paid", StringComparison.OrdinalIgnoreCase);

        existing.InvoiceNumber = request.InvoiceNumber;
        existing.CustomerName = request.CustomerName;
        existing.Amount = request.Amount;
        existing.Status = request.Status;

        var updated = await _invoiceRepository.UpdateAsync(existing);
        if (!updated)
        {
            return null;
        }

        var nowPaid = string.Equals(existing.Status, "Paid", StringComparison.OrdinalIgnoreCase);
        if (!wasPaid && nowPaid)
        {
            _ = Task.Run(() => LogPaymentConfirmationAsync(existing));
        }

        return ToResponse(existing);
    }

    public Task<bool> DeleteAsync(int invoiceId)
    {
        return _invoiceRepository.DeleteAsync(invoiceId);
    }

    private async Task LogPaymentConfirmationAsync(Invoice invoice)
    {
        await Task.Delay(500);
        _logger.LogInformation(
            "Payment confirmation logged for InvoiceId={InvoiceId}, InvoiceNumber={InvoiceNumber}, Amount={Amount}",
            invoice.InvoiceId,
            invoice.InvoiceNumber,
            invoice.Amount);
    }

    private static InvoiceResponse ToResponse(Invoice invoice)
    {
        return new InvoiceResponse
        {
            InvoiceId = invoice.InvoiceId,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerName = invoice.CustomerName,
            Amount = invoice.Amount,
            Status = invoice.Status,
            CreatedDate = invoice.CreatedDate
        };
    }
}
