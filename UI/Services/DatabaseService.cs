using Microsoft.Data.SqlClient;
using UI.ViewModels.DataModels;

namespace UI.Services
{
    /// <include file='../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="DatabaseService"]/*'/>
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

        /// <summary>
        /// Retrieves all active bank accounts from the database.
        /// </summary>
        /// <returns>An enumerable of AccountInfo for active accounts.</returns>
        /// <exception cref="SqlException">Thrown if the query execution fails.</exception>
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

        /// <summary>
        /// Retrieves all closed bank accounts from the database.
        /// </summary>
        /// <returns>An enumerable of AccountInfo for closed accounts, including the closing date.</returns>
        /// <exception cref="SqlException">Thrown if the query execution fails.</exception>
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

        /// <summary>
        /// Retrieves information about the last 20 client commands from the database.
        /// </summary>
        /// <returns>An enumerable of ClientInfo containing client IP, last command, timestamp, and result status.</returns>
        /// <exception cref="SqlException">Thrown if the query execution fails.</exception>
        public IEnumerable<ClientInfo> GetClientInformation()
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
}