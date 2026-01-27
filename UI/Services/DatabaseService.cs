using Microsoft.Data.SqlClient;
using UI.ViewModels;
using UI.ViewModels.DataModels;

namespace UI.Services;

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
    
    public IEnumerable<ClientInfo> GetClientInfos()
    {
        using var cmd = new SqlCommand(
            @"select top 20 client_ip, command, arguments, executed_at, result_status
                  from view_ui_client_activity
                  order by executed_at desc",
            connection
        );

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            yield return new ClientInfo
            {
                Ip = (string)reader["client_ip"],
                LastCommand = $"{reader["command"]} {reader["arguments"]}".Trim(),
                Timestamp = (DateTime)reader["executed_at"],
                Status = (string)reader["result_status"]
            };
        }
    }
}