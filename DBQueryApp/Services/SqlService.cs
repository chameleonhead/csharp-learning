using Microsoft.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace DBQueryApp.Services;

public class SqlService
{
    private readonly string _connectionString;

    public SqlService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<TableDefinition> GetSchema()
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var schema = connection.GetSchema("Tables");
        var tables = new List<TableDefinition>();

        foreach (DataRow row in schema.Rows)
        {
            var tableSchema = (string)row["TABLE_SCHEMA"];
            var tableName = (string)row["TABLE_NAME"];
            var columns = new List<ColumnDefinition>();

            // カラム一覧を取得
            using var columnCmd = new SqlCommand($"SELECT TOP 0 * FROM [{tableSchema}].[{tableName}]", connection);
            using var reader = columnCmd.ExecuteReader();

            var schemaTable = reader.GetSchemaTable();
            foreach (DataRow colRow in schemaTable.Rows)
            {
                var columnName = (string)colRow["ColumnName"];
                columns.Add(new ColumnDefinition { Name = columnName });
            }
            tables.Add(new TableDefinition
            {
                Name = $"{tableSchema}.{tableName}",
                Columns = columns
            });
        }

        return tables;
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

    public class TableDefinition
    {
        public required string Name { get; set; }
        public required IReadOnlyList<ColumnDefinition> Columns { get; set; }
    }

    public class ColumnDefinition
    {
        public required string Name { get; set; }
    }
}
