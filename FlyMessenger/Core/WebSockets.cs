using System;
using System.Linq;
using WebSocket4Net;
using FlyMessenger.Core.Utils;
using FlyMessenger.HTTP;
using Newtonsoft.Json;
using Application = System.Windows.Application;

namespace FlyMessenger.Core
{
    public class WebSockets
    {
        private WebSocket _webSocket;
        private bool _isConnected;
        private bool _isReconnecting;

        public WebSockets()
        {
            Connect();
        }

        private void Connect()
        {
            if (_webSocket != null && _webSocket.State == WebSocketState.Open && _isConnected) return;
            _webSocket = new WebSocket("ws://localhost:8000/ws?token=" + HttpClientBase.GetToken())
            {
                EnableAutoSendPing = true,
                AutoSendPingInterval = 30
            };
            _webSocket.Opened += OnWebSocketOpened;
            _webSocket.Closed += OnWebSocketClosed;
            _webSocket.Error += OnWebSocketError;
            _webSocket.MessageReceived += OnWebSocketMessageReceived;
            _webSocket.Open();
        }

        private void OnWebSocketOpened(object? sender, EventArgs e)
        {
            _isConnected = true;
            _isReconnecting = false;
        }

        private void OnWebSocketClosed(object? sender, EventArgs e)
        {
            _isConnected = false;
            if (_isReconnecting) return;
            _isReconnecting = true;
            Reconnect();
        }

        private void OnWebSocketError(object? sender, EventArgs e)
        {
            _isReconnecting = true;
            Reconnect();
        }

        private void OnWebSocketMessageReceived(object? sender, MessageReceivedEventArgs e)
        {
            if (!_isConnected || _isReconnecting || _webSocket.State == WebSocketState.Closed) return;

            dynamic json = JsonConvert.DeserializeObject(e.Message)!;
            if (json == null) return;

            var type = (string)json.type;
            if (string.IsNullOrEmpty(type)) return;
            // Handle message based on message type
            switch (type)
            {
                case "RECEIVE_MESSAGE":
                    if (MainWindow.MainViewModel.MyProfile.Settings
                        .ChatsNotificationsEnabled)
                    {
                        if ((string)json.dialog.isNotificationsEnabled == "false") return;
                        NotificationsManager.SendNotification(json);
                    }
                    Application.Current.Dispatcher.Invoke(
                        () => { MainWindow.MainViewModel.SendMessage(json.message, (string)json.message.dialogId, json.dialog); }
                    );
                    break;
                case "READ_MESSAGE":
                    Application.Current.Dispatcher.Invoke(
                        () => { MainWindow.MainViewModel.ReadMessage((string)json.messageId, (string)json.dialogId); }
                    );
                    break;
                case "TOGGLE_ONLINE_STATUS":
                    var dialog = MainWindow.MainViewModel.Dialogs.FirstOrDefault(x => x.User.Id == (string)json.userId);
                    if (dialog == null) return;
                    dialog.User.IsOnline = (bool)json.status;
                    dialog.User.LastActivity = (string)json.lastActivity;
                    break;
                case "USER_BLOCKED":
                    MainWindow.MainViewModel.SelectedDialogTextBoxVisibility = !(bool)json.isBlocked;
                    break;
                case "USER_LOGOUT":
                    if (json.success == null)
                    {
                        HttpClientBase.SetToken("");
                        Application.Current.Dispatcher.Invoke(
                            () =>
                            {
                                if (Application.Current.MainWindow is not MainWindow window) return;
                                window.CloseWindow();
                            }
                        );
                    }
                    break;
            }
        }

        public void Send(string message)
        {
            while (true)
            {
                if (_isConnected && _webSocket.State == WebSocketState.Open)
                {
                    if (_isReconnecting) return;
                    _webSocket.Send(message);
                }
                else
                {
                    continue;
                }
                break;
            }
        }

        private void Reconnect(Action? callback = null)
        {
            // Task.Delay(1000).ContinueWith(t => Connect());
            _isReconnecting = true;
            Connect();
            callback?.Invoke();
        }
    }
}
