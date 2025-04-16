using Microsoft.Data.SqlClient;

namespace LIBCORE.DataRepository.Helper
{
    internal class MsSqlDatabase
    {
        internal MsSqlDatabase(SqlConnection sqlConnection, SqlCommand sqlCommand, SqlDataAdapter sqlDataAdapter) 
        {
            SqlConnection = sqlConnection;
            SqlCommand = sqlCommand;
            SqlDataAdapter = sqlDataAdapter;
        }

        internal SqlConnection SqlConnection { get; private set; }
        internal SqlCommand SqlCommand { get; private set; }
        internal SqlDataAdapter SqlDataAdapter { get; private set; }
    }
}
