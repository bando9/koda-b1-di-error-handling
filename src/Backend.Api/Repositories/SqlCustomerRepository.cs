// using Backend.Api.Models;
// using Microsoft.Data.SqlClient;

// namespace Backend.Api.Repositories;

// public sealed class SqlCustomerRepository : ICustomerRepository
// {
//     private readonly string _connectionString;

//     public SqlCustomerRepository(IConfiguration configuration)
//     {
//         _connectionString=configuration.GetConnectionString("ecommercedb") ?? throw new InvalidOperationException("Connection string to ecommerce needed");
//     }

//     public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken)
//     {
//         const string sql = """
//             SELECT Name, Email, Phone, Address FROM dbo.Customer ORDER BY Name DESC;
//         """;

//         List<Customer> customers = new();
//         await using var connection = new SqlConnection(_connectionString);

//         await connection.OpenAsync(cancellationToken);

//         await using var command = new SqlCommand(sql, connection);

//         await using var reader = await command.ExecuteReaderAsync(cancellationToken);

//         while(await reader.ReadAsync(cancellationToken))
//         {
//             customers.Add(ReadPro);
//         }

//         return customers;
//     }

//     public Task<Product?> GetByCodeAsync(string productCode, CancellationToken cancellationToken)
//     {
//         throw new NotImplementedException();
//     }

//     public Task<bool> TryAddAsync(Product product, CancellationToken cancellationToken)
//     {
//         throw new NotImplementedException();
//     }

//     public Task<bool> TryDeleteAsync(string productCode, CancellationToken cancellationToken)
//     {
//         throw new NotImplementedException();
//     }

//     public Task<bool> TryUpdateAsync(Product product, CancellationToken cancellationToken)
//     {
//         throw new NotImplementedException();
//     }


//     // * ========== METHOD ==========
//     public static Customer ReadCustomer(SqlDataReader reader)
//     {
//         return new Customer(
// reader.GetString(0),
// reader.GetString(1),
// reader.GetString(2),
// reader.GetString(3)
//         );
//     }
// }