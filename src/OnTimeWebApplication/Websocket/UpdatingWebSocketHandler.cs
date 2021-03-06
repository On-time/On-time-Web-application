﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OnTimeWebApplication.Websocket
{
    public class UpdatingWebSocketHandler : WebSocketHandler
    {
        public UpdatingWebSocketHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
        }

        public override Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var initialMessage = JsonConvert.DeserializeObject<InitialConnectMessage>(Encoding.UTF8.GetString(buffer, 0, buffer.Length));
            var socketList = WebSocketConnectionManager.GetSocketListById(initialMessage.SubjectId + initialMessage.Section);
            socketList.Add(socket);

            return Task.CompletedTask;
        }
    }

    public class InitialConnectMessage
    {
        public string SubjectId { get; set; }
        public byte Section { get; set; }
    }
}
