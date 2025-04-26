using Microsoft.Data.SqlClient;
using System.Data;

namespace DBQueryApp.Services;

public class SqlService
{
    private readonly string _connectionString;

    public SqlService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DataTable ExecuteQuery(string query)
    {
        var table = new DataTable();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            using (var command = new SqlCommand(query, connection))
            using (var adapter = new SqlDataAdapter(command))
            {
                adapter.Fill(table);
            }
        }

        return table;
    }
}
