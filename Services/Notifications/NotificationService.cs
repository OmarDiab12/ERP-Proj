using ERP.DTOs.Invoices;
using ERP.Hubs;
using ERP.Services.Interfaces.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace ERP.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task BroadcastDueRemindersAsync(IEnumerable<InvoiceDueReminderDTO> reminders)
        {
            return _hubContext.Clients.All.SendAsync("dueReminders", reminders);
        }

        public Task BroadcastMessageAsync(string topic, object payload)
        {
            return _hubContext.Clients.All.SendAsync(topic, payload);
        }
    }
}
