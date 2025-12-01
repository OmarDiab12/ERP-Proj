using ERP.DTOs.Invoices;

namespace ERP.Services.Interfaces.Notifications
{
    public interface INotificationService
    {
        Task BroadcastDueRemindersAsync(IEnumerable<InvoiceDueReminderDTO> reminders);
        Task BroadcastMessageAsync(string topic, object payload);
    }
}
