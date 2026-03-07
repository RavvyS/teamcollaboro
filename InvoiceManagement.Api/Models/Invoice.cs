namespace InvoiceManagement.Api.Models;

public class Invoice
{
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Draft";
    public DateTime CreatedDate { get; set; }
}
