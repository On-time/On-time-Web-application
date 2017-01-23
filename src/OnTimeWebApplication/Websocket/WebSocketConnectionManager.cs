using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace OnTimeWebApplication.Websocket
{
    public class WebSocketConnectionManager
    {
        private ConcurrentDictionary<string, List<WebSocket>> _sockets = new ConcurrentDictionary<string, List<WebSocket>>();

        public List<WebSocket> GetSocketListById(string id)
        {
            List<WebSocket> socketList = null;
            
            if (!_sockets.TryGetValue(id, out socketList))
            {
                socketList = new List<WebSocket>();
                _sockets.TryAdd(id, socketList);
            }

            return socketList;
        }

        public ConcurrentDictionary<string, List<WebSocket>> GetAll()
        {
            return _sockets;
        }

        public string GetId(WebSocket socket)
        {
            var id = (from pair in _sockets
                      where pair.Value.Contains(socket)
                      select pair.Key).FirstOrDefault();

            return id;
        }
        public void AddSocket(WebSocket socket, string id)
        {
            List<WebSocket> sockets = null;
            var flag = _sockets.TryGetValue(id, out sockets);

            if (flag)
            {
                sockets.Add(socket);
            }
            else
            {
                _sockets.TryAdd(id, new List<WebSocket> { socket });
            }
        }

        public async Task RemoveSocket(WebSocket socket)
        {
            var keyValue = (from pair in _sockets
                              where pair.Value.Contains(socket)
                              select pair)
                              .FirstOrDefault();

            if (keyValue.Value == null)
            {
                await CloseSocket(socket);
                return;
            }

            List<WebSocket> socketList = keyValue.Value;

            socketList.Remove(socket);

            await CloseSocket(socket);

            if (socketList.Count == 0)
            {
                _sockets.TryRemove(keyValue.Key, out socketList);
            }
        }

        private string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }

        private Task CloseSocket(WebSocket socket)
        {
            return socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                    statusDescription: "Closed by the WebSocketManager",
                                    cancellationToken: CancellationToken.None);
        }
    }
}
