using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlyMessenger.Core.Utils;
using FlyMessenger.HTTP;
using FlyMessenger.MVVM.Model;
using Newtonsoft.Json;

namespace FlyMessenger.Core
{
    public class WebSockets
    {
        private readonly ClientWebSocket _clientWebSocket;

        public WebSockets()
        {
            _clientWebSocket = new ClientWebSocket();
        }

        public async Task ConnectAsync(Uri uri)
        {
            if (_clientWebSocket.State == WebSocketState.Open)
            {
                await CloseAsync();
            }
            await _clientWebSocket.ConnectAsync(uri, CancellationToken.None);
            await ListenAsync();
        }

        private async Task ListenAsync()
        {
            while (true)
            {
                // Handle incoming close frame and send close frame
                if (_clientWebSocket.State == WebSocketState.CloseReceived)
                {
                    await Task.Delay(1000);
                    await ConnectAsync(new Uri("ws://localhost:8000/ws?token=" + HttpClientBase.GetToken()));
                    break;
                }
                
                var buffer = new byte[1024 * 4];
                var result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                
                // var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                // dynamic json = JsonConvert.DeserializeObject(message)!;
                // var type = json.type;
                // if (type == null) continue;
                // if (type == "RECEIVE_MESSAGE")
                // {
                //     NotificationsManager.SendNotification(json);
                // }
                // else if (type == "SEND_MESSAGE")
                // {
                //     MessageBox.Show("I sent a message");
                // }
                // else if (type == "READ_MESSAGE")
                // {
                //     MessageBox.Show("I read a message");
                // }
                // else if (type == "TOGGLE_ONLINE_STATUS")
                // {
                //     MessageBox.Show("I toggled my online status");
                // }
                // else if (type == "TYPING")
                // {
                //     MessageBox.Show("I'm typing");
                // }
                // else if (type == "UNTYPING")
                // {
                //     MessageBox.Show("I'm untyping");
                // }
                // else if (type == "DESTROY_SESSION")
                // {
                //     MessageBox.Show("I destroyed a session");
                // }
                // else if (type == "USER_BLOCKED")
                // {
                //     MessageBox.Show("I blocked a user");
                // }
                // else if (type == "USER_LOGOUT")
                // {
                //     MessageBox.Show("I logged out a user");
                // }
            }
        }

        public async Task SendAsync(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            var segment = new ArraySegment<byte>(buffer);
            await _clientWebSocket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task<string> ReceiveAsync()
        {
            var buffer = new byte[1024 * 4];
            var segment = new ArraySegment<byte>(buffer);
            var result = await _clientWebSocket.ReceiveAsync(segment, CancellationToken.None);
            return Encoding.UTF8.GetString(buffer, 0, result.Count);
        }

        private async Task CloseAsync()
        {
            await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
    }
}
