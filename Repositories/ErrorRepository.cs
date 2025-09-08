using ERP.Models;
using ERP.Repositories.Interfaces;

namespace ERP.Repositories
{
    public class ErrorRepository : IErrorRepository
    {
        private readonly ERPDBContext _context;

        public ErrorRepository(ERPDBContext context)
        {
            _context = context;
        }

        public async Task<bool> LogErrorAsync(string message, string functionName, string stackTrace = "", int? userId = null)
        {
            try
            {
                var errorLog = new Error
                {
                    ErrorMessage = message,
                    FunctionName = functionName,
                    StackTrace = stackTrace,
                    UserId = userId,
                    LoggedAt = DateTime.UtcNow
                };

                await _context.AddAsync(errorLog);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception fileEx)
            {
                try
                {
                    string logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");
                    if (!Directory.Exists(logDirectory))
                        Directory.CreateDirectory(logDirectory);

                    string filePath = Path.Combine(logDirectory, $"error_{DateTime.UtcNow:yyyyMMdd}.log");

                    string logEntry = $"""
            -------------------------
            Time (UTC): {DateTime.UtcNow:O}
            Function: {functionName}
            Error Message: {message}
            Stack Trace: {stackTrace}
            DB Logging Exception: {fileEx.Message}
            -------------------------

            """;

                    await File.AppendAllTextAsync(filePath, logEntry);
                }
                catch
                {
                }

                return false;
            }

        }

    }
}
