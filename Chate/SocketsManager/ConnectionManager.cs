using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Chate.SocketsManager
{
    public class ConnectionManager {
        private ConcurrentDictionary<string, WebSocket> _connections = new ConcurrentDictionary<string, WebSocket>();

        public WebSocket GetSocketById(string Id) =>
            _connections.FirstOrDefault(x => x.Key == Id).Value;

        public ConcurrentDictionary<string, WebSocket> GetAllConnections() =>
            _connections;

        public string GetId(WebSocket webSocket) =>
            _connections.FirstOrDefault(x => x.Value == webSocket).Key;

        public async Task RemoveSocketAsync(string id) {
            _connections.TryRemove(id, out var socket);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "socket connetction closed",
                CancellationToken.None);
        }

        public void AddSocket(WebSocket socket) =>
            _connections.TryAdd(GetConnectionId(), socket);

        private string GetConnectionId() =>
            Guid.NewGuid().ToString("N");
    }
}