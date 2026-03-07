using Dapper;
using InvoiceManagement.Api.Infrastructure;
using InvoiceManagement.Api.Models;
using InvoiceManagement.Api.Repositories.Interfaces;

namespace InvoiceManagement.Api.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public InvoiceRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateAsync(Invoice invoice)
    {
        const string sql = @"
            INSERT INTO Invoices (InvoiceNumber, CustomerName, Amount, Status, CreatedDate)
            VALUES (@InvoiceNumber, @CustomerName, @Amount, @Status, @CreatedDate);
            SELECT CAST(SCOPE_IDENTITY() AS int);";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<int>(sql, invoice);
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        const string sql = @"
            SELECT InvoiceId, InvoiceNumber, CustomerName, Amount, Status, CreatedDate
            FROM Invoices
            ORDER BY CreatedDate DESC";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Invoice>(sql);
    }

    public async Task<Invoice?> GetByIdAsync(int invoiceId)
    {
        const string sql = @"
            SELECT InvoiceId, InvoiceNumber, CustomerName, Amount, Status, CreatedDate
            FROM Invoices
            WHERE InvoiceId = @InvoiceId";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Invoice>(sql, new { InvoiceId = invoiceId });
    }

    public async Task<bool> UpdateAsync(Invoice invoice)
    {
        const string sql = @"
            UPDATE Invoices
            SET InvoiceNumber = @InvoiceNumber,
                CustomerName = @CustomerName,
                Amount = @Amount,
                Status = @Status
            WHERE InvoiceId = @InvoiceId";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, invoice);
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int invoiceId)
    {
        const string sql = "DELETE FROM Invoices WHERE InvoiceId = @InvoiceId";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { InvoiceId = invoiceId });
        return rows > 0;
    }
}
