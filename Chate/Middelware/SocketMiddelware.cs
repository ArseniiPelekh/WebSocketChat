using Chate.Handlers;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Chate.Middelware
{
    public class SocketMiddelware
    {
        private readonly RequestDelegate _next;
        private SocketHandler Handler { get; set; }

        public SocketMiddelware(RequestDelegate next, SocketHandler handler) {
            _next = next;
            Handler = handler;
        }

        private async Task Receive(WebSocket webSocket, Action<WebSocketReceiveResult, byte[]> messageHandle) {
            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open) {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                messageHandle(result, buffer);
            }
        }

        public async Task InvokeAsync(HttpContext context) {
            if (!context.WebSockets.IsWebSocketRequest) {
                return;
            }
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            await Receive(socket, async (result, buffer) => {
                if (result.MessageType == WebSocketMessageType.Text) {
                    await Handler.Receive(socket, result, buffer);
                } 
                else if (result.MessageType == WebSocketMessageType.Close) {
                    await Handler.OnDisconnected(socket);
                }
            });
        }
    }
}