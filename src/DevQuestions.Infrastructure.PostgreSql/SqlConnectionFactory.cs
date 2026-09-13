using System.Data;
using DevQuestions.Application.Database;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DevQuestions.Infrastructure.PostgreSql;

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly IConfiguration _configuration;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection Create()
    {
        var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        return connection;
    }
}
