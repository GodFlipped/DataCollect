
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataCollect.Core.Configure
{
    public class ChatHub : Hub
    {
      

        public async Task SendMessage(string Message)
        {
            await Clients.All.SendAsync("ReceiveMessage", Message);
        }
    }
}
