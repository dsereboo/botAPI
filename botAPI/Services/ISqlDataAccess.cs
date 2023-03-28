namespace botAPI.Services
{
    public interface ISqlDataAccess
    {
        Task<IEnumerable<T>> ReadData<T, U>(string storedProcedure, U parameters, string connectionId = "Default");
        Task<int> WriteData<T>(string storedProcedure, T parameters, string connectionId = "Default");
    }
}