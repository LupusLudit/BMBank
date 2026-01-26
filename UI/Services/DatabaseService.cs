using Microsoft.Data.SqlClient;
using UI.ViewModels;

namespace UI;

public class DatabaseService
{
    private readonly SqlConnection connection;

    public DatabaseService(SqlConnection connection)
    {
        this.connection = connection;
        if (connection.State != System.Data.ConnectionState.Open)
        {
            connection.Open();
        }
    }

    public IEnumerable<AccountInfo> GetActiveAccounts()
    {
        using var cmd = new SqlCommand("SELECT account_number, balance, created_at_date FROM view_active_accounts", connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            yield return new AccountInfo
            {
                AccountNumber = (int)reader["account_number"],
                Balance = (long)reader["balance"],
                CreatedAt = (DateTime)reader["created_at_date"]
            };
        }
    }

    public IEnumerable<AccountInfo> GetClosedAccounts()
    {
        using var cmd = new SqlCommand("SELECT account_number, balance, created_at_date, closed_at_date FROM view_closed_accounts", connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            yield return new AccountInfo
            {
                AccountNumber = (int)reader["account_number"],
                Balance = (long)reader["balance"],
                CreatedAt = (DateTime)reader["created_at_date"],
                ClosedAt = reader["closed_at_date"] as DateTime?
            };
        }
    }
}