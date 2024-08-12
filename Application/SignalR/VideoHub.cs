using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.SignalR
{
    public class VideoHub : Hub
    {
        public async Task SendVideoFrame(byte[] videoFrame)
        {
           
            await Clients.All.SendAsync("ReceiveVideoFrame", videoFrame);
        }
    }
}
