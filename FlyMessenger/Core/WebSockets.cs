using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using FlyMessenger.Controllers;
using FlyMessenger.Converters;
using WebSocket4Net;
using FlyMessenger.Core.Utils;
using FlyMessenger.HTTP;
using FlyMessenger.MVVM.Model;
using FlyMessenger.Resources.Languages;
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
            DialogModel? dialog;
            TokenSettings? tokenSettings;
            switch (type)
            {
                case "RECEIVE_MESSAGE":
                    Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                            if (Application.Current.MainWindow is not MainWindow mainWindow) return;
                            if (!mainWindow.IsActive)
                            {
                                if (MainWindow.MainViewModel.MyProfile.Settings
                                    .ChatsNotificationsEnabled)
                                {
                                    if ((bool)json.dialog.isNotificationsEnabled)
                                        NotificationsManager.SendNotification(json);
                                }
                            }
                            MainWindow.MainViewModel.SendMessage(json.message, (string)json.message.dialogId, json.dialog);
                        }
                    );
                    break;
                case "READ_MESSAGE":
                    Application.Current.Dispatcher.Invoke(
                        () => { MainWindow.MainViewModel.ReadMessage((string)json.messageId, (string)json.dialogId); }
                    );
                    break;
                case "TOGGLE_ONLINE_STATUS":
                    dialog = MainWindow.MainViewModel.Dialogs.FirstOrDefault(x => x.User.Id == (string)json.userId);
                    if (dialog == null) return;
                    dialog.User.IsOnline = (bool)json.status;
                    dialog.User.LastActivity = (string)json.lastActivity;
                    Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                            if (Application.Current.MainWindow is not MainWindow window) return;
                            window.ChatBoxListView.Items.Refresh();

                            if (dialog.User.IsOnline)
                            {
                                window.LastActivityTextBlock.Text = lang.online;
                            }
                            else
                            {
                                // Set last activity to dialog.User.LastActivity with LastActivityConverter converter
                                var converter = new LastActivityConverter();
                                var value = dialog.User.LastActivity;
                                var targetType = typeof(string);
                                object? parameter = null;
                                var culture = CultureInfo.CurrentCulture;
                                var result = converter.Convert(value, targetType, parameter, culture);
                                window.LastActivityTextBlock.Text = result as string;
                            }
                        }
                    );
                    break;
                case "TYPING":
                    Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                            var dialogT = MainWindow.MainViewModel.Dialogs.FirstOrDefault(x => x.Id == (string)json.dialogId);
                            if (dialogT == null) return;
                            dialogT.Typing = lang.typing;
                            if (Application.Current.MainWindow is not MainWindow window) return;
                            window.ChatBoxListView.Items.Refresh();
                        }
                    );
                    break;
                case "UNTYPING":
                    Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                            var dialogT = MainWindow.MainViewModel.Dialogs.FirstOrDefault(x => x.Id == (string)json.dialogId);
                            if (dialogT == null) return;
                            dialogT.Typing = null;
                            if (Application.Current.MainWindow is not MainWindow window) return;
                            window.ChatBoxListView.Items.Refresh();
                        }
                    );
                    break;
                case "USER_BLOCKED":
                    MainWindow.MainViewModel.SelectedDialogTextBoxVisibility = !(bool)json.isBlocked;
                    break;
                case "USER_LOGOUT":
                    if (json.success == null)
                    {
                        HttpClientBase.SetToken("");
                        tokenSettings = new TokenSettings();
                        tokenSettings.Delete();
                        Application.Current.Dispatcher.Invoke(
                            () =>
                            {
                                if (Application.Current.MainWindow is not MainWindow window) return;
                                window.CloseWindow();
                                var loginWindow = new LoginWindow();
                                loginWindow.Show();
                            }
                        );
                    }
                    break;
                case "DELETE_USER":
                    HttpClientBase.SetToken("");
                    tokenSettings = new TokenSettings();
                    tokenSettings.Delete();
                    Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                            if (Application.Current.MainWindow is not MainWindow window) return;
                            window.CloseWindow();
                            var loginWindow = new LoginWindow();
                            loginWindow.Show();
                        }
                    );
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
