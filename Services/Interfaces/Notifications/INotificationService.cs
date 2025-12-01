using ERP.DTOs.Invoices;
using ERP.DTOs.Inventory;

namespace ERP.Services.Interfaces.Notifications
{
    public interface INotificationService
    {
        Task BroadcastDueRemindersAsync(IEnumerable<InvoiceDueReminderDTO> reminders);
        Task BroadcastLowStockAsync(IEnumerable<InventoryLowStockAlertDTO> alerts);
        Task BroadcastMessageAsync(string topic, object payload);
    }
}
