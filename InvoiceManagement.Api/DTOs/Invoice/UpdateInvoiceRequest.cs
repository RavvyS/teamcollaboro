using System.ComponentModel.DataAnnotations;

namespace InvoiceManagement.Api.DTOs.Invoice;

public class UpdateInvoiceRequest
{
    [Required]
    [MaxLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string CustomerName { get; set; } = string.Empty;

    [Range(0, 999999999)]
    public decimal Amount { get; set; }

    [Required]
    [RegularExpression("Draft|Paid|Cancelled", ErrorMessage = "Status must be Draft, Paid or Cancelled")]
    public string Status { get; set; } = "Draft";
}
