using BMBank.Src.DatabaseInteraction.Core.DBEntities;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BMBank.Src.DatabaseInteraction.Core.DAO
{
    public class AccountDAO : DAOBase<Account>
    {
        public AccountDAO(SqlConnection connection) : base(connection)
        {
        }
        
        /// <summary>
        /// Creates a new account in the database.
        /// </summary>
        /// <returns>The account number of the newly created account.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the account number could not be retrieved after creation.
        /// </exception>
        public int CreateAccount()
        {
            using (SqlCommand command = CreateCommand("create_account"))
            {
                command.CommandType = CommandType.StoredProcedure;
                var returnParameter = command.Parameters.Add("@return_value", SqlDbType.Int);
                returnParameter.Direction = ParameterDirection.ReturnValue;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return (int)reader["account_number"];
                    }
                }
                throw new InvalidOperationException("Failed to retrieve account number.");
            }
        }

        /// <summary>
        /// Deposits a specified amount into a given account.
        /// </summary>
        /// <param name="accountNumber">The account number to deposit into.</param>
        /// <param name="amount">The amount to deposit.</param>
        public void Deposit(int accountNumber, long amount)
        {
            SqlCommand command = CreateCommand("deposit_account");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@account_number", accountNumber);
            command.Parameters.AddWithValue("@amount", amount);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Withdraws a specified amount from a given account.
        /// </summary>
        /// <param name="accountNumber">The account number to withdraw from.</param>
        /// <param name="amount">The amount to withdraw.</param>
        public void Withdraw(int accountNumber, long amount)
        {
            SqlCommand command = CreateCommand("withdraw_account");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@account_number", accountNumber);
            command.Parameters.AddWithValue("@amount", amount);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Retrieves the balance of a specified account.
        /// </summary>
        /// <param name="accountNumber">The account number to query.</param>
        /// <returns>The current balance of the account.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the account is not found.</exception>
        public long GetBalance(int accountNumber)
        {
            SqlCommand command = CreateCommand("get_account_balance");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@account_number", accountNumber);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return (long)reader["balance"];
                }
            }

            throw new InvalidOperationException("Account not found.");
        }

        /// <summary>
        /// Removes the specified account from the database.
        /// </summary>
        /// <param name="accountNumber">The account number to remove.</param>
        public void RemoveAccount(int accountNumber)
        {
            SqlCommand command = CreateCommand("remove_account");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@account_number", accountNumber);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Retrieves the total sum of money across all accounts in the bank.
        /// </summary>
        /// <returns>Total amount in the bank.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the total amount cannot be retrieved.
        /// </exception>
        public long GetTotalAmount()
        {
            SqlCommand command = CreateCommand("select total_amount from view_bank_total_amount");
            var result = command.ExecuteScalar();

            if(result != DBNull.Value && result != null)
            {
                return (long)result;
            }
            else
            {
                throw new InvalidOperationException("Failed to retrieve total amount.");
            }
        }

        /// <summary>
        /// Retrieves the total number of clients in the bank.
        /// </summary>
        /// <returns>Total client count.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the client count cannot be retrieved.
        /// </exception>
        public int GetClientCount()
        {
            SqlCommand command = CreateCommand("select client_count from view_bank_client_count");
            var result = command.ExecuteScalar();

            if (result != DBNull.Value && result != null)
            {
                return (int)result;
            }
            else
            {
                throw new InvalidOperationException("Failed to retrieve total amount.");
            }
        }
    }
}