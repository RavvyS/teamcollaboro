using Dapper;
using InvoiceManagement.Api.Infrastructure;
using InvoiceManagement.Api.Models;
using InvoiceManagement.Api.Repositories.Interfaces;

namespace InvoiceManagement.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public UserRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        const string sql = @"
            SELECT Id, Name, Email, PasswordHash, Role, CreatedAt
            FROM Users
            WHERE Email = @Email";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Email = email });
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT Id, Name, Email, PasswordHash, Role, CreatedAt
            FROM Users
            WHERE Id = @Id";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
    }

    public async Task<int> CreateAsync(User user)
    {
        const string sql = @"
            INSERT INTO Users (Name, Email, PasswordHash, Role, CreatedAt)
            VALUES (@Name, @Email, @PasswordHash, @Role, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS int);";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<int>(sql, user);
    }
}
