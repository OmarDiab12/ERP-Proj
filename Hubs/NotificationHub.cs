using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ERP.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
    }
}
