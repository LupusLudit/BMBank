using BMBank.Src.DatabaseInteraction.Core.DBEntities;
using Microsoft.Data.SqlClient;

namespace BMBank.Src.DatabaseInteraction.Core.DAO;

public class ClientCommandLogDAO : DAOBase<ClientCommandLog>
{
    public ClientCommandLogDAO(SqlConnection connection) : base(connection)
    {
    }

    public void Insert(string ip, string clientCommand, string arguments, string resultStatus)
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
}