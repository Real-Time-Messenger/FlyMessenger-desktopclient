using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using FlyMessenger.Controllers;
using FlyMessenger.Core;
using FlyMessenger.MVVM.Model;
using FlyMessenger.Properties;
using FlyMessenger.Resources.Languages;
using FlyMessenger.Resources.Settings;
using Newtonsoft.Json;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace FlyMessenger.MVVM.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public ObservableCollection<DialogModel> Dialogs { get; set; } = new ObservableCollection<DialogModel>();
        public ObservableCollection<DialogModel?> DialogsInSearch { get; set; } = new ObservableCollection<DialogModel?>();
        public ObservableCollection<DialogModel> MessagesInSearch { get; set; } = new ObservableCollection<DialogModel>();
        public ObservableCollection<UserModel?> UsersInSearch { get; set; } = new ObservableCollection<UserModel?>();
        public static ObservableCollection<SessionsModel>? Sessions { get; set; }
        public RelayCommand SendCommand { get; set; }
        public RelayCommand ClearSearchBox { get; set; }

        // Add a flag to check if messages are loaded or it's last time to load messages
        private bool _allItemsLoaded;
        private bool _isLastTime;

        // Add a flag to check if messages are loading
        private bool _isFetching;
        
        // Add a keeper of previous dialog
        private DialogModel? _previousDialog;

        private SearchModel _searchResult;

        public SearchModel SearchResult
        {
            get => _searchResult;
            set
            {
                _searchResult = value;
                OnPropertyChanged();
            }
        }

        private SearchModel _searchResultInDialog;

        public SearchModel SearchResultInDialog
        {
            get => _searchResultInDialog;
            set
            {
                _searchResultInDialog = value;
                OnPropertyChanged();
            }
        }

        private DialogInSearchModel _selectedDialogInSearch;

        public DialogInSearchModel SelectedDialogInSearch
        {
            get => _selectedDialogInSearch;
            set
            {
                _selectedDialogInSearch = value;
                OnPropertyChanged();
            }
        }

        private bool _dialogsNotFoundVisibility = true;

        public bool DialogsNotFoundVisibility
        {
            get => _dialogsNotFoundVisibility;
            set
            {
                _dialogsNotFoundVisibility = value;
                OnPropertyChanged();
            }
        }

        private bool _messagesNotFoundVisibility = true;

        public bool MessagesNotFoundVisibility
        {
            get => _messagesNotFoundVisibility;
            set
            {
                _messagesNotFoundVisibility = value;
                OnPropertyChanged();
            }
        }

        private bool _usersNotFoundVisibility = true;

        public bool UsersNotFoundVisibility
        {
            get => _usersNotFoundVisibility;
            set
            {
                _usersNotFoundVisibility = value;
                OnPropertyChanged();
            }
        }

        public async void Search(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            SearchBoxClearVisibility = true;
            ActiveChatVisibility = false;
            ActiveChatVisibilityNone = true;
            DialogsInSearch.Clear();
            MessagesInSearch.Clear();
            UsersInSearch.Clear();
            
            SearchResult = await ControllerBase.SearchController.Search(text);
            MessagesNotFoundVisibility = SearchResult.Messages.Length == 0;
            DialogsNotFoundVisibility = SearchResult.Dialogs?.Length == 0;
            UsersNotFoundVisibility = SearchResult.Users?.Length == 0;

            foreach (var dialog in SearchResult.Messages)
            {
                MessagesInSearch.Add(dialog);
            }

            if (SearchResult.Dialogs == null || SearchResult.Users == null)
                return;
            foreach (var dialog in SearchResult.Dialogs)
            {
                DialogsInSearch.Add(dialog);

                // Заменить dialog в Dialogs на dialog из SearchResult.Dialogs
                Dialogs.Remove(Dialogs.FirstOrDefault(d => d.Id == dialog.Id)!);
                Dialogs.Add(dialog);
            }
            foreach (var user in SearchResult.Users)
            {
                UsersInSearch.Add(user);
            }

            foreach (var dialog in DialogsInSearch)
            {
                if (dialog == null)
                    continue;
                ValidateMessages(dialog.Messages);
            }
        }

        public async void SearchInDialog(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            SearchBoxClearVisibility = true;
            MessagesInSearch.Clear();

            SearchResultInDialog = await ControllerBase.SearchController.SearchInDialog(text, SelectedDialog.Id);
            MessagesNotFoundVisibility = SearchResultInDialog.Messages.Length == 0;
            foreach (var dialog in SearchResultInDialog.Messages)
            {
                MessagesInSearch.Add(dialog);
            }
        }

        private bool _selectedDialogTextBoxVisibility;

        public bool SelectedDialogTextBoxVisibility
        {
            get => _selectedDialogTextBoxVisibility;
            set
            {
                _selectedDialogTextBoxVisibility = value;
                OnPropertyChanged();
            }
        }

        private DialogModel _selectedDialog;

        public DialogModel SelectedDialog
        {
            get => _selectedDialog;
            set
            {
                _selectedDialog = value;
                ValidateSelectedDialog();
                OnPropertyChanged();
            }
        }

        private UserModel _selectedUser;

        public UserModel SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
            }
        }

        private void ValidateSelectedDialog()
        {
            if (SelectedDialog == null!)
            {
                SelectedDialogTextBoxVisibility = false;
                return;
            }

            if (SelectedDialog.User.IsBlocked)
            {
                SelectedDialogTextBoxVisibility = false;
            }
            else if (SelectedDialog.IsMeBlocked)
            {
                SelectedDialogTextBoxVisibility = false;
            }
            else
            {
                SelectedDialogTextBoxVisibility = true;
            }
        }

        private bool _borderVisibility;

        public bool BorderVisibility
        {
            get => _borderVisibility;
            set
            {
                _borderVisibility = value;
                OnPropertyChanged();
            }
        }

        private bool _messagesLoadingVisibility;

        public bool MessagesLoadingVisibility
        {
            get => _messagesLoadingVisibility;
            set
            {
                _messagesLoadingVisibility = value;
                OnPropertyChanged();
            }
        }

        private bool _scrollDownButtonVisibility;

        public bool ScrollDownButtonVisibility
        {
            get => _scrollDownButtonVisibility;
            set
            {
                _scrollDownButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        private bool _activeChatVisibility = true;

        public bool ActiveChatVisibility
        {
            get => _activeChatVisibility;
            set
            {
                _activeChatVisibility = value;
                OnPropertyChanged();
            }
        }

        private bool _activeChatVisibilityNone;

        public bool ActiveChatVisibilityNone
        {
            get => _activeChatVisibilityNone;
            set
            {
                _activeChatVisibilityNone = value;
                OnPropertyChanged();
            }
        }

        private bool _noMessagesVisibility = true;

        public bool NoMessagesVisibility
        {
            get => _noMessagesVisibility;
            set
            {
                _noMessagesVisibility = value;
                OnPropertyChanged();
            }
        }

        private bool _searchBoxClearVisibility;

        public bool SearchBoxClearVisibility
        {
            get => _searchBoxClearVisibility;
            set
            {
                _searchBoxClearVisibility = value;
                OnPropertyChanged();
            }
        }

        private bool _isCloseButtonVisible;

        public bool IsCloseButtonVisible
        {
            get => _isCloseButtonVisible;
            set
            {
                _isCloseButtonVisible = value;
                OnPropertyChanged();
            }
        }

        private int _unreadMessagesCount;

        public int UnreadMessagesCount
        {
            get => _unreadMessagesCount;
            set
            {
                _unreadMessagesCount = value;
                OnPropertyChanged();
            }
        }

        private int _sessionsCount;

        public int SessionsCount
        {
            get => _sessionsCount;
            set
            {
                _sessionsCount = value;
                OnPropertyChanged();
            }
        }

        private int _blockedUsersCount;

        public int BlockedUsersCount
        {
            get => _blockedUsersCount;
            set
            {
                _blockedUsersCount = value;
                OnPropertyChanged();
            }
        }

        private UserModel _myProfile;

        public UserModel MyProfile
        {
            get => _myProfile;
            set
            {
                _myProfile = value;
                OnPropertyChanged();
            }
        }

        private SettingsPage _currentPage;

        public SettingsPage CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        private string _checkedAction;

        public string CheckedAction
        {
            get => _checkedAction;
            set
            {
                _checkedAction = value;
                OnPropertyChanged();
            }
        }

        private bool _isEstonianChecked;

        public bool IsEstonianChecked
        {
            get => _isEstonianChecked;
            set
            {
                _isEstonianChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _isRussianChecked;

        public bool IsRussianChecked
        {
            get => _isRussianChecked;
            set
            {
                _isRussianChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _isEnglishChecked;

        public bool IsEnglishChecked
        {
            get => _isEnglishChecked;
            set
            {
                _isEnglishChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _isAnyoneChecked;

        public bool IsAnyoneChecked
        {
            get => _isAnyoneChecked;
            set
            {
                _isAnyoneChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _isNobodyChecked;

        public bool IsNobodyChecked
        {
            get => _isNobodyChecked;
            set
            {
                _isNobodyChecked = value;
                OnPropertyChanged();
            }
        }

        private SessionsModel? _currentSession;

        public SessionsModel? CurrentSession
        {
            get => _currentSession;
            set
            {
                _currentSession = value;
                OnPropertyChanged();
            }
        }

        private bool _autoStartupEnabled;

        public bool AutoStartupEnabled
        {
            get => _autoStartupEnabled;
            set
            {
                _autoStartupEnabled = value;
                OnPropertyChanged();
            }
        }
        
        private bool IsDateDifferent(MessageModel firstMessage, MessageModel secondMessage, int? minMinutes = null)
        {
            var firstSentAt = DateTimeOffset.Parse(firstMessage.SentAt).Date;
            var secondSentAt = DateTimeOffset.Parse(secondMessage.SentAt).Date;

            if (minMinutes == null) return firstSentAt != secondSentAt;

            var firstSentAtTime = DateTimeOffset.Parse(firstMessage.SentAt).TimeOfDay;
            var secondSentAtTime = DateTimeOffset.Parse(secondMessage.SentAt).TimeOfDay;

            return firstSentAt != secondSentAt || firstSentAtTime - secondSentAtTime > TimeSpan.FromMinutes(minMinutes.Value);
        }

        private void ValidateMessages(Collection<MessageModel> messages, bool isScrolling = false)
        {
            if (messages.Count == 0) return;

            for (var i = 0; i < messages.Count; i++)
            {
                var message = messages[i];
                if (i == 0)
                {
                    message.NewDate = true;
                    SetPhotoVisibility(message);
                }

                var pastMessage = i == 0 ? null : messages[i - 1];
                if (pastMessage == null)
                {
                    if (isScrolling)
                        SelectedDialog.Messages.Insert(i, message);
                    continue;
                }

                message.NewDate = IsDateDifferent(message, pastMessage);
                message.IsMyMessage = message.Sender.Id == MyProfile.Id;

                if (i == messages.Count - 1)
                {
                    if (isScrolling)
                        SelectedDialog.Messages.Insert(i, message);
                    SetPhotoVisibility(message);
                    break;
                }
                var nextMessage = messages[i + 1];
                if (message.IsMyMessage == true)
                {
                    if (nextMessage.Sender.Id != MyProfile.Id)
                    {
                        message.IsMyMessage = false;
                        SetPhotoVisibility(message);
                    }
                }
                else
                {
                    if (nextMessage.Sender.Id == MyProfile.Id)
                    {
                        SetPhotoVisibility(message);
                    }
                }

                if (IsDateDifferent(message, pastMessage, 10))
                {
                    SetPhotoVisibility(pastMessage);
                }

                if (isScrolling)
                    SelectedDialog.Messages.Insert(i, message);
            }
        }

        private void ValidateMessage(MessageModel message, DialogModel dialog)
        {
            if (dialog.Messages is { Count: > 0 })
            {
                var lastMessage = dialog.Messages.Last();

                message.NewDate = IsDateDifferent(message, lastMessage);

                if (IsDateDifferent(message, lastMessage, 10))
                {
                    SetPhotoVisibility(lastMessage);
                }

                message.IsMyMessage = message.Sender.Id == MyProfile.Id;
                if (lastMessage.Sender.Id == MyProfile.Id && message.Sender.Id == MyProfile.Id)
                {
                    lastMessage.IsMyPhotoVisible = false;
                    lastMessage.IsMyMessage = true;
                    message.IsMyPhotoVisible = true;
                }
                else if (lastMessage.Sender.Id != MyProfile.Id && message.Sender.Id != MyProfile.Id)
                {
                    lastMessage.IsCompanionPhotoVisible = false;
                    message.IsCompanionPhotoVisible = true;
                }
                else
                {
                    SetPhotoVisibility(message);
                }

                if (Application.Current.MainWindow is not MainWindow window) return;
                window.MessagesListView.Items.Refresh();
                dialog.Messages.Add(message);

                if (SelectedDialog != null! && SelectedDialog.Id == dialog.Id)
                    if (window.MessagesScrollViewer.VerticalOffset > window.MessagesScrollViewer.ExtentHeight -
                        window.MessagesScrollViewer.ViewportHeight - 320 || message.IsMyMessage == true)
                        window.MessagesListView.ScrollIntoView(SelectedDialog.Messages.Last());

                dialog.LastMessage = message;
                if (message.IsRead || message.Sender.Id == MyProfile.Id) return;
                dialog.UnreadMessages++;
                UnreadMessagesCount = Dialogs.Count(x => x.UnreadMessages != 0);
                window.ChatBoxListView.Items.Refresh();
            }
            else
            {
                message.NewDate = true;
                SetPhotoVisibility(message);
                dialog.Messages.Add(message);
                dialog.LastMessage = message;
            }
        }

        private void SetPhotoVisibility(MessageModel message)
        {
            if (message.Sender.Id == MyProfile.Id)
            {
                message.IsMyPhotoVisible = true;
            }
            else
            {
                message.IsCompanionPhotoVisible = true;
            }
        }

        public void SendMessage(dynamic message, string dialogId, dynamic dialog)
        {
            var newMessage = JsonConvert.DeserializeObject<MessageModel>(message.ToString());
            var existDialog = Dialogs.FirstOrDefault(x => x.Id == dialogId);
            if (existDialog == null)
            {
                var newDialog = JsonConvert.DeserializeObject<DialogModel>(dialog.ToString());
                Dialogs.Add(newDialog);
                UnreadMessagesCount = Dialogs.Count(x => x.UnreadMessages != 0);

                if (Application.Current.MainWindow is not MainWindow window) return;
                window.ChatBoxListView.Items.Refresh();
                ValidateMessage(newMessage, newDialog);
                NoMessagesVisibility = false;
                return;
            }
            ValidateMessage(newMessage, existDialog);
        }

        public void ReadMessage(string messageId, string dialogId)
        {
            if (Application.Current.MainWindow is not MainWindow window) return;
            var dialog = Dialogs.FirstOrDefault(x => x.Id == dialogId);
            var message = dialog?.Messages.FirstOrDefault(x => x.Id == messageId);
            if (message == null) return;
            message.IsRead = true;
            if (message.Sender.Id != MyProfile.Id) return;
            window.MessagesListView.Items.Refresh();
        }

        public async void LoadMoreItems(UIElement? firstItem)
        {
            var dialog = SelectedDialog;
            if (dialog != _previousDialog)
            {
                _allItemsLoaded = false;
                _isFetching = false;
                _isLastTime = false;
                _previousDialog = dialog;
            }
            if (_allItemsLoaded || _isFetching) return;

            MessagesLoadingVisibility = true;

            _isFetching = true;

            var newMessages = await ControllerBase.DialogController.LoadMoreMessages(
                dialog.Id,
                dialog.Messages.Count
            );
            if (newMessages.Count != 100)
            {
                _allItemsLoaded = true;
                MessagesLoadingVisibility = false;
            }

            if (_isLastTime)
            {
                _isFetching = false;
                return;
            }

            var scrollViewer = (ScrollViewer)Application.Current.MainWindow?.FindName("MessagesScrollViewer")!;
            ValidateMessages(newMessages, true);
            scrollViewer.UpdateLayout();
            // Scroll to last item
            if (SelectedDialog.Messages.Count > 99)
                scrollViewer.ScrollToVerticalOffset(firstItem?.TranslatePoint(new Point(0, 0), scrollViewer).Y ?? 0);

            if (_allItemsLoaded)
            {
                _isLastTime = true;
                MessagesLoadingVisibility = false;
            }

            MessagesLoadingVisibility = false;
            _isFetching = false;
            
        }

        public MainViewModel()
        {
            CurrentPage = SettingsManager.DefaultPage;
            MyProfile = ControllerBase.UserController.GetMyProfile();

            var sessions = ControllerBase.UserController.GetMySessions();
            Sessions = new ObservableCollection<SessionsModel>(sessions);
            SessionsCount = MyProfile.Sessions.Count;
            // Get current session
            CurrentSession = Sessions.FirstOrDefault(x => x.Current == true);
            // Remove current session from Sessions
            if (CurrentSession != null)
            {
                Sessions.Remove(CurrentSession);
            }
            MyProfile.Sessions = Sessions;

            BlockedUsersCount = MyProfile.BlackList.Count;

            var dialogs = ControllerBase.DialogController.GetDialogs();

            // Get all dialogs
            if (dialogs.Length != 0)
            {
                foreach (var dialog in dialogs)
                {
                    Dialogs.Add(dialog);
                }

                NoMessagesVisibility = false;

                // Sort by MessageDateTime and MessageAnchor
                Dialogs = new ObservableCollection<DialogModel>(
                    Dialogs.OrderByDescending(x => x.IsPinned).ThenByDescending(x => x.LastMessage?.SentAt)
                );

                // Count how many MessagesCount are
                UnreadMessagesCount = Dialogs.Count(x => x.UnreadMessages != 0);

                foreach (var dialog in Dialogs)
                {
                    ValidateMessages(dialog.Messages);
                }
            }
            else
            {
                NoMessagesVisibility = true;
                UnreadMessagesCount = 0;
            }

            switch (Settings.Default.LanguageCode)
            {
                case "et-EE":
                    IsEstonianChecked = true;
                    break;
                case "ru-RU":
                    IsRussianChecked = true;
                    break;
                case "en-US":
                    IsEnglishChecked = true;
                    break;
            }

            switch (MyProfile.Settings.LastActivityMode)
            {
                case true:
                    IsAnyoneChecked = true;
                    CheckedAction = lang.anyone;
                    break;
                case false:
                    IsNobodyChecked = true;
                    CheckedAction = lang.nobody;
                    break;
            }

            AutoStartupEnabled = Settings.Default.RunOnStartupAllowed;

            ClearSearchBox = new RelayCommand(
                _ =>
                {
                    if (Application.Current.MainWindow is not MainWindow window) return;
                    ResetSearchFields(window);
                    ToggleSearchControlVisibility(window, false);
                    ClearCollections();
                    OnPropertyChanged(nameof(window.SearchBox.Text));
                },
                _ => true
            );

            void ResetSearchFields(MainWindow window)
            {
                window.SearchBox.Text = string.Empty;
                window.SearchBoxSecond.Text = string.Empty;
            }

            void ToggleSearchControlVisibility(MainWindow window, bool isSearching)
            {
                window.SearchAllPanel.Visibility = isSearching ? Visibility.Visible : Visibility.Collapsed;
                window.SearchInChatPanel.Visibility = Visibility.Collapsed;
                window.DialogsPanel.Visibility = isSearching ? Visibility.Collapsed : Visibility.Visible;
                window.ClearSearchBoxButton.Visibility = isSearching ? Visibility.Visible : Visibility.Collapsed;
                window.SearchBoxSecond.Visibility = isSearching ? Visibility.Visible : Visibility.Collapsed;
                window.SearchBox.Visibility = isSearching ? Visibility.Collapsed : Visibility.Visible;
                ActiveChatVisibilityNone = true;
                ActiveChatVisibility = false;
            }

            void ClearCollections()
            {
                DialogsInSearch.Clear();
                UsersInSearch.Clear();
                MessagesInSearch.Clear();
            }
        }
    }
}
