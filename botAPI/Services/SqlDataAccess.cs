using System.Data.SqlClient;
using System.Data;
using Dapper;

namespace botAPI.Services
{
    public class SqlDataAccess : ISqlDataAccess
    {
        //read and write methods to database
        private readonly IConfiguration _config;

        public SqlDataAccess(IConfiguration config)
        {
            _config = config;
        }

        public async Task<int> WriteData<T>(
                string storedProcedure,
                T parameters,
                string connectionId = "Default"
            )
        {
            //open db connection
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));
            
            var res = await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            return res;
        }

        public async Task<IEnumerable<T>> ReadData<T, U>(
        string storedProcedure,
        U parameters,
        string connectionId = "Default"
        )
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));
            var response = await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
            return response;
        }
    }
}
