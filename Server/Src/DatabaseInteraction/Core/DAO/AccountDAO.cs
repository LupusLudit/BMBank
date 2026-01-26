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

        public void Deposit(int accountNumber, long amount)
        {
            SqlCommand command = CreateCommand("deposit_account");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@account_number", accountNumber);
            command.Parameters.AddWithValue("@amount", amount);
            command.ExecuteNonQuery();
        }

        public void Withdraw(int accountNumber, long amount)
        {
            SqlCommand command = CreateCommand("withdraw_account");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@account_number", accountNumber);
            command.Parameters.AddWithValue("@amount", amount);
            command.ExecuteNonQuery();
        }

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

        public void RemoveAccount(int accountNumber)
        {
            SqlCommand command = CreateCommand("remove_account");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@account_number", accountNumber);
            command.ExecuteNonQuery();
        }

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