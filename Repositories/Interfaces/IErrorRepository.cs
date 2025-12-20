namespace ERP.Repositories.Interfaces
{
    public interface IErrorRepository
    {
        Task<bool> LogErrorAsync(string message, string functionName, string stackTrace = "", int? userId = null);
    }
}
