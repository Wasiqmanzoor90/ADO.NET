using MVC_Studio.Interfaces;
using Microsoft.Data.SqlClient;

namespace MVC_Studio.Service
{
    public class SqlService : ISqlService
    {
        private readonly string _connectionString;

        public SqlService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString") ?? throw new InvalidOperationException("Failed to load connection string");
        }

  

        public async Task<bool> RegisterUserAsync(string name, string email, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Users (Name, Email, Password) VALUES (@Name, @Email, @Password)";
                var commond = new SqlCommand(query, connection);

                commond.Parameters.AddWithValue("@Name", name);
                commond.Parameters.AddWithValue("@Email", email);
                commond.Parameters.AddWithValue("@Password", password);

                try
                {
                    await connection.OpenAsync();
                    int result = await commond.ExecuteNonQueryAsync();
                    return result > 0; // Returns true if the user was inserted
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error", ex);
                }
            }

        }




        //Is user exits

        public async Task<bool> UserExistsAsync(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);

                try
                {
                    await connection.OpenAsync();
                    // Use ExecuteScalarAsync to get the count of users
                    int count = (int)await command.ExecuteScalarAsync();
                    return count > 0; // If count is greater than 0, the user exists
                }
                catch (Exception ex)
                {
                    // Throw an exception with more context instead of NotImplementedException
                    throw new InvalidOperationException("Error checking if user exists", ex);
                }
            }
        }




        //Login Service
        public async Task<bool> LoginAsync(string Email, string Password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT Password FROM Users WHERE Email = @Email";
                var commond = new SqlCommand(query, connection);
                commond.Parameters.AddWithValue("@Email", Email); // Parameterized query to avoid SQL injection

                try
                {
                    // Open database connection asynchronously
                    await connection.OpenAsync();

                    // Execute the query to fetch the password for the given email
                    var result = await commond.ExecuteScalarAsync();

                    if (result == null)
                    {
                        // No result means user doesn't exist
                        return false;
                    }

                    // Convert the result to string (i.e., the stored password)
                    string storedPassword = result.ToString();

                    // If passwords match, return true
                    return Password == storedPassword; // Plain text comparison (works only for un-hashed passwords)
                }
                catch (Exception ex)
                {
                    // Catch and log any exceptions for debugging
                    Console.WriteLine($"Error: {ex.Message}");
                    throw new InvalidOperationException("Error validating password", ex);
                }
            }
        }


    }
}

