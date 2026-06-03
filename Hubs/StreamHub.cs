using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace PylonStream.Hubs
{
    public class StreamHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            // Optional: push current camera state immediately on connection
            try
            {
                // We'll let the client query it or push on next tick
            }
            catch
            {
                // Ignore errors
            }
        }
    }
}
