using Application.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;

namespace GlassesApp.Controllers
{
  
        public class VideoFrameController : ControllerBase
        {
            private readonly IHubContext<VideoHub> _hubContext;

            public VideoFrameController(IHubContext<VideoHub> hubContext)
            {
                _hubContext = hubContext;
            }

            [HttpPost("stream")]
            public async Task<IActionResult> StreamVideo()
            {
                if (!HttpContext.WebSockets.IsWebSocketRequest)
                {
                    return BadRequest("WebSocket request expected.");
                }

                WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
              
                await ProcessVideoFrames(webSocket);

                return new EmptyResult();
            }

            private async Task ProcessVideoFrames(WebSocket webSocket)
            {
                var buffer = new byte[1024 * 4];
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);

                while (!result.CloseStatus.HasValue)
                {
                    // Send the received video frame to SignalR hub
                    await _hubContext.Clients.All.SendAsync(
                        "ReceiveVideoFrame", buffer);

                    // Receive next video frame
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>
                        (buffer), CancellationToken.None
                        
                        );
                }
            }
        }
}
