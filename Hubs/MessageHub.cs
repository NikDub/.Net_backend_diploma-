using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Hubs
{
    public class MessageHub : Hub
    {
        public async Task Echo(Question message)
        {
            await Clients.All.SendAsync("send",message);
        }
    }
}
