using Microsoft.Data.SqlClient;

namespace BookOrg.Src.Logic.Connection
{
    /// <include file='../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="IConnectionFactory"]/*'/>
    public interface IConnectionFactory
    {
        SqlConnection? CreateConnection();
    }
}
