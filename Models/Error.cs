namespace ERP.Models
{
    public class Error
    {
        public int Id { get; set; }
        public string? ErrorMessage { get; set; }
        public string? FunctionName { get; set; }
        public string? StackTrace { get; set; }
        public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
        public string? Source { get; set; }
        public int? UserId { get; set; }
    }
}
