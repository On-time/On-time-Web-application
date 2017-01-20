﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace OnTimeWebApplication.Websocket
{
    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate _next;
        private WebSocketHandler _webSocketHandler { get; set; }

        public WebSocketManagerMiddleware(RequestDelegate next,
                                          WebSocketHandler webSocketHandler)
        {
            _next = next;
            _webSocketHandler = webSocketHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next(context);
                return;
            }

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            //await _webSocketHandler.OnConnected(socket);

            await Receive(socket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    await _webSocketHandler.ReceiveAsync(socket, result, buffer);
                    return;
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocketHandler.OnDisconnected(socket);
                    return;
                }

            });

            //TODO - investigate the Kestrel exception thrown when this is the last middleware
            //await _next.Invoke(context);
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                       cancellationToken: CancellationToken.None);
                var realMsgLength = BitConverter.ToInt32(buffer.Take(4).ToArray(), 0);
                int receivedData = result.Count - 4;

                if (realMsgLength != receivedData)
                {
                    // remaining space in array
                    var remainBuffer = buffer.Length - result.Count;
                    // start index of empty space in array, base 0 index, so can use Count as value
                    var startIndex = result.Count;

                    do
                    {
                        result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer, startIndex, remainBuffer),
                                                       cancellationToken: CancellationToken.None);
                        startIndex += result.Count;
                        remainBuffer -= result.Count;
                        receivedData += result.Count;
                    } while (receivedData != realMsgLength);
                }

                handleMessage(result, buffer);
            }
        }
    }
}
