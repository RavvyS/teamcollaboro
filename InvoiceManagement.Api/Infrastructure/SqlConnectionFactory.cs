using System.Data;
using Microsoft.Data.SqlClient;

namespace InvoiceManagement.Api.Infrastructure;

public class SqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is not configured");
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
