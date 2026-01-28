using Microsoft.Data.SqlClient;
using System.Configuration;

namespace BookOrg.Src.Logic.Connection
{
    /// <include file='../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="SqlServerConnectionFactory"]/*'/>
    public class SqlServerConnectionFactory : IConnectionFactory
    {
        public SqlServerConnectionFactory() { }

        /// <summary>
        /// Creates and opens a connection to the SQL Server database.
        /// </summary>
        /// <returns>An open SqlConnection object, or null if the connection failed.</returns>
        public SqlConnection? CreateConnection()
        {
            SqlConnection? connection = null;

            string? dataSource = ConfigurationManager.AppSettings["DataSource"];
            string? database = ConfigurationManager.AppSettings["Database"];
            string? login = ConfigurationManager.AppSettings["Login"];
            string? password = ConfigurationManager.AppSettings["Password"];

            ValidateConfig(dataSource, database, login, password);

            string connectionString =
                $"Server={dataSource};" +
                $"Database={database};" +
                $"User Id={login};" +
                $"Password={password};" +
                "TrustServerCertificate=True;";
                // Add "Trusted_Connection=True;" for local testing

            var connectionAttempt = new SqlConnection(connectionString);
            connectionAttempt.Open();

            connection = connectionAttempt;

            return connection;
        }

        /// <summary>
        /// Validates the database configuration parameters.
        /// </summary>
        /// <param name="ds">The data source (server address).</param>
        /// <param name="db">The database name.</param>
        /// <param name="user">The user login ID.</param>
        /// <param name="pwd">The user password.</param>
        /// <exception cref="ApplicationException">Thrown when configuration is missing or invalid.</exception>
        private void ValidateConfig(string? ds, string? db, string? user, string? pwd)
        {
            if (string.IsNullOrWhiteSpace(ds) || string.IsNullOrWhiteSpace(db) ||
                string.IsNullOrWhiteSpace(user) || pwd == null)
            {
                throw new ApplicationException("Database configuration is missing or invalid");
            }
        }
    }
}