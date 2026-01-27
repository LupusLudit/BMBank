using BMBank.Src.Common;
using BMBank.Src.DatabaseInteraction.Core.DBEntities;
using Microsoft.Data.SqlClient;

namespace BMBank.Src.DatabaseInteraction.Core.DAO;

public class ClientCommandLogDAO : DAOBase<ClientCommandLog>
{
    public ClientCommandLogDAO(SqlConnection connection) : base(connection)
    {
    }

    /// <summary>
    /// Inserts a new client command log into the database and ensures only the latest 50 logs are kept.
    /// </summary>
    /// <param name="ip">The IP address of the client.</param>
    /// <param name="clientCommand">The command key (e.g., "AB", "BN").</param>
    /// <param name="arguments">The arguments passed with the command.</param>
    /// <param name="resultStatus">The result status of the command ("OK" or "ERROR").</param>
    public void Insert(string ip, string clientCommand, string arguments, string resultStatus)
    {
        try
        {
            using var tx = connection.BeginTransaction();

            using var cmd = CreateCommand(
                @"INSERT INTO client_command_log 
                  (client_ip, command, arguments, result_status)
                  VALUES (@ip, @cmd, @args, @status)"
            );
            cmd.Transaction = tx;

            cmd.Parameters.AddWithValue("@ip", ip);
            cmd.Parameters.AddWithValue("@cmd", clientCommand);
            cmd.Parameters.AddWithValue("@args", arguments);
            cmd.Parameters.AddWithValue("@status", resultStatus);

            cmd.ExecuteNonQuery();
        
            using var cleanupCmd = CreateCommand(
                @"DELETE FROM client_command_log
                  WHERE id NOT IN (
                      SELECT TOP 50 id
                      FROM client_command_log
                      ORDER BY executed_at DESC
                  )"
            );
            cleanupCmd.Transaction = tx;
            cleanupCmd.ExecuteNonQuery();

            tx.Commit();
        }
        catch (SqlException ex)
        {
            Logger.Error($"Database error while inserting client command log: {ex.Message}");
            throw; 
        }
        catch (InvalidOperationException ex)
        {
            Logger.Error($"Transaction error in ClientCommandLogDAO.Insert: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Logger.Error($"Unexpected error in ClientCommandLogDAO.Insert: {ex.Message}");
            throw;
        }
    }
}