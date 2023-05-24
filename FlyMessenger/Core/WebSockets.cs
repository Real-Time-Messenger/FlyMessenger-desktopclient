using System;
using System.Globalization;
using System.Linq;
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
    /// <summary>
    /// WebSockets.
    /// </summary>
    public class WebSockets
    {
        private WebSocket _webSocket;
        private bool _isConnected;
        private bool _isReconnecting;

        public WebSockets()
        {
            Connect();
        }

        // Connect to WebSocket.
        private void Connect()
        {
            // Check if WebSocket is already connected.
            if (_webSocket != null && _webSocket.State == WebSocketState.Open && _isConnected) return;
            
            // Create new WebSocket.
            _webSocket = new WebSocket("ws://localhost:8000/ws?token=" + HttpClientBase.GetToken())
            {
                EnableAutoSendPing = true,
                AutoSendPingInterval = 30
            };
            
            // Add WebSocket events.
            _webSocket.Opened += OnWebSocketOpened;
            _webSocket.Closed += OnWebSocketClosed;
            _webSocket.Error += OnWebSocketError;
            _webSocket.MessageReceived += OnWebSocketMessageReceived;
            _webSocket.Open();
        }

        // Check if WebSocket is opened.
        private void OnWebSocketOpened(object? sender, EventArgs e)
        {
            _isConnected = true;
            _isReconnecting = false;
        }

        // Check if WebSocket is closed.
        private void OnWebSocketClosed(object? sender, EventArgs e)
        {
            _isConnected = false;
            if (_isReconnecting) return;
            _isReconnecting = true;
            Reconnect();
        }

        // Check if WebSocket is in error state.
        private void OnWebSocketError(object? sender, EventArgs e)
        {
            _isReconnecting = true;
            Reconnect();
        }

        // Receive message from WebSocket.
        private void OnWebSocketMessageReceived(object? sender, MessageReceivedEventArgs e)
        {
            // Check if WebSocket is connected.
            if (!_isConnected || _isReconnecting || _webSocket.State == WebSocketState.Closed) return;

            // Deserialize message.
            dynamic json = JsonConvert.DeserializeObject(e.Message)!;
            if (json == null) return;

            // Get message type.
            var type = (string)json.type;
            if (string.IsNullOrEmpty(type)) return;
            
            // Handle message based on message type
            DialogModel? dialog;
            TokenSettings? tokenSettings;
            switch (type)
            {
                // Receive message.
                case "RECEIVE_MESSAGE":
                    Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                            // Check if main window is active.
                            if (Application.Current.MainWindow is not MainWindow mainWindow) return;
                            if (!mainWindow.IsActive)
                            {
                                // Check if notifications are enabled.
                                if (MainWindow.MainViewModel.MyProfile.Settings
                                    .ChatsNotificationsEnabled)
                                {
                                    // Check if notifications are enabled for this dialog.
                                    if ((bool)json.dialog.isNotificationsEnabled)
                                        NotificationsManager.SendNotification(json);
                                }
                            }
                            // Send message to dialog.
                            MainWindow.MainViewModel.GetMessage(json.message, (string)json.message.dialogId, json.dialog);
                        }
                    );
                    break;
                
                // Read message.
                case "READ_MESSAGE":
                    Application.Current.Dispatcher.Invoke(
                        () => { MainWindow.MainViewModel.ReadMessage((string)json.messageId, (string)json.dialogId); }
                    );
                    break;
                
                // Toggle online status.
                case "TOGGLE_ONLINE_STATUS":
                    
                    // Get dialog.
                    dialog = MainWindow.MainViewModel.Dialogs.FirstOrDefault(x => x.User.Id == (string)json.userId);
                    if (dialog == null) return;
                    
                    // Set online status.
                    dialog.User.IsOnline = (bool)json.status;
                    dialog.User.LastActivity = (string)json.lastActivity;
                    
                    // Update last activity.
                    Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                            if (Application.Current.MainWindow is not MainWindow window) return;
                            
                            // Refresh dialogs list.
                            window.ChatBoxListView.Items.Refresh();

                            // Set last activity to "online" if user is online.
                            if (dialog.User.IsOnline)
                            {
                                window.LastActivityTextBlock.Text = lang.online;
                            }
                            else
                            {
                                // Set last activity to last activity if user is offline.
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
                
                // Toggle typing status.
                case "TYPING":
                    Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                            // Get dialog.
                            var dialogT = MainWindow.MainViewModel.Dialogs.FirstOrDefault(x => x.Id == (string)json.dialogId);
                            if (dialogT == null) return;
                            
                            // Set typing status.
                            dialogT.Typing = lang.typing;
                            if (Application.Current.MainWindow is not MainWindow window) return;
                            
                            // Refresh dialogs list.
                            window.ChatBoxListView.Items.Refresh();
                        }
                    );
                    break;
                
                // Remove typing status.
                case "UNTYPING":
                    Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                            // Get dialog.
                            var dialogT = MainWindow.MainViewModel.Dialogs.FirstOrDefault(x => x.Id == (string)json.dialogId);
                            if (dialogT == null) return;
                            
                            // Remove typing status.
                            dialogT.Typing = null;
                            if (Application.Current.MainWindow is not MainWindow window) return;
                            
                            // Refresh dialogs list.
                            window.ChatBoxListView.Items.Refresh();
                        }
                    );
                    break;
                
                // Check if user is blocked.
                case "USER_BLOCKED":
                    // Remove dialog input box if user is blocked.
                    MainWindow.MainViewModel.DialogInputVisibility = !(bool)json.isBlocked;
                    break;
                
                // Check if user is logged out.
                case "USER_LOGOUT":
                    if (json.success == null)
                    {
                        // Remove token.
                        HttpClientBase.SetToken("");
                        tokenSettings = new TokenSettings();
                        tokenSettings.Delete();
                        
                        // Show login window.
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
                
                // Delete user.
                case "DELETE_USER":
                    // Remove token.
                    HttpClientBase.SetToken("");
                    tokenSettings = new TokenSettings();
                    tokenSettings.Delete();
                    
                    // Show login window.
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

        // Send message to WebSocket.
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

        // Reconnect to WebSocket.
        private void Reconnect(Action? callback = null)
        {
            // Task.Delay(1000).ContinueWith(t => Connect());
            _isReconnecting = true;
            Connect();
            callback?.Invoke();
        }
    }
}
