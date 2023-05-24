using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using FlyMessenger.Controllers;
using FlyMessenger.Core;
using FlyMessenger.MVVM.Model;
using FlyMessenger.Properties;
using FlyMessenger.Resources.Languages;
using FlyMessenger.Resources.Settings;
using Newtonsoft.Json;
using Application = System.Windows.Application;

namespace FlyMessenger.MVVM.ViewModels
{
    /// <summary>
    /// Main view model.
    /// </summary>
    public class MainViewModel : ObservableObject
    {
        #region Collections

        public ObservableCollection<DialogModel> Dialogs { get; set; } = new ObservableCollection<DialogModel>();
        public ObservableCollection<DialogModel?> DialogsInSearch { get; set; } = new ObservableCollection<DialogModel?>();
        public ObservableCollection<DialogModel> MessagesInSearch { get; set; } = new ObservableCollection<DialogModel>();
        public ObservableCollection<DialogModel?> UsersInSearch { get; set; } = new ObservableCollection<DialogModel?>();
        public static ObservableCollection<SessionsModel?>? Sessions { get; set; }

        #endregion

        // Last items in search object
        public LastItemModel LastItemsInSearch { get; set; } = new LastItemModel();

        // Clear search box command
        public RelayCommand ClearSearchBox { get; set; }

        // Add a flag to check if messages are loaded or it's last time to load messages
        private bool _allItemsLoaded;
        private bool _isLastTime;

        // Add a flag to check if messages are loading
        private bool _isFetching;

        // Add a keeper of previous dialog
        private DialogModel? _previousDialog;

        // Search result model
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

        // Search result in all dialogs model
        private SearchModel _dialogSearchResult;

        public SearchModel DialogSearchResult
        {
            get => _dialogSearchResult;
            set
            {
                _dialogSearchResult = value;
                OnPropertyChanged();
            }
        }

        // Search result in specific dialog model
        private DialogInSearchModel _selectedDialogSearch;

        public DialogInSearchModel SelectedDialogSearch
        {
            get => _selectedDialogSearch;
            set
            {
                _selectedDialogSearch = value;
                OnPropertyChanged();
            }
        }

        #region Visibility

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

        private bool _lastItemsNotFoundVisibility = true;

        public bool LastItemsNotFoundVisibility
        {
            get => _lastItemsNotFoundVisibility;
            set
            {
                _lastItemsNotFoundVisibility = value;
                OnPropertyChanged();
            }
        }

        private bool _dialogInputVisibility;

        public bool DialogInputVisibility
        {
            get => _dialogInputVisibility;
            set
            {
                _dialogInputVisibility = value;
                OnPropertyChanged();
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

        // Active Chat Visibility Hidden
        private bool _activeChatVisHidden;

        public bool ActiveChatVisHidden
        {
            get => _activeChatVisHidden;
            set
            {
                _activeChatVisHidden = value;
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

        #endregion

        #region Selected items

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

        private DialogModel _selectedItemInSearch;

        public DialogModel SelectedItemInSearch
        {
            get => _selectedItemInSearch;
            set
            {
                _selectedItemInSearch = value;
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

        #endregion

        #region Count properties

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

        #endregion

        // My profile property
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

        // Settings page property
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

        // Checked action property
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

        #region Language checkboxes properties

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

        #endregion

        #region Last activity checkboxes properties

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

        #endregion

        // Current session property
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

        // Auto startup property
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

        /// <summary>
        /// Search method.
        /// </summary>
        /// <param name="text">Text to search.</param>
        public async Task Search(string text)
        {
            // If the text is empty, exit the method without performing any search.
            // Clear search results for all types of objects (dialogs, messages, users) and update the visibility of related UI elements.
            if (string.IsNullOrEmpty(text))
                return;
            SearchBoxClearVisibility = true;
            DialogsInSearch.Clear();
            MessagesInSearch.Clear();
            UsersInSearch.Clear();

            // Perform a search and get the result.
            SearchResult = await ControllerBase.SearchController.Search(text);
            MessagesNotFoundVisibility = SearchResult.Messages?.Length == 0;
            DialogsNotFoundVisibility = SearchResult.Dialogs?.Length == 0;
            UsersNotFoundVisibility = SearchResult.Users?.Length == 0;

            // If the result is null, exit the method.
            if (SearchResult.Dialogs == null || SearchResult.Users == null || SearchResult.Messages == null)
                return;

            // Add the result to the corresponding collections.
            foreach (var dialog in SearchResult.Messages)
            {
                MessagesInSearch.Add(dialog);
            }

            // Sort dialogs by last message sent time
            SearchResult.Dialogs = SearchResult.Dialogs.OrderByDescending(d => d.LastMessage?.SentAt).ToArray();

            // Add dialogs to the collection
            foreach (var dialog in SearchResult.Dialogs)
            {
                DialogsInSearch.Add(dialog);

                Dialogs.Remove(Dialogs.FirstOrDefault(d => d.Id == dialog.Id)!);
                Dialogs.Add(dialog);
            }

            // Add users to the collection without duplicates
            var distinctUsers = SearchResult.Users.Distinct().ToArray();
            foreach (var user in distinctUsers)
            {
                UsersInSearch.Add(user);
            }

            if (DialogsInSearch.Count == 0)
                return;

            // Validate dialogs that are in search
            foreach (var dialog in DialogsInSearch)
            {
                if (dialog?.Messages != null) ValidateMessages(dialog.Messages);
            }
        }

        /// <summary>
        /// Search inside dialog method.
        /// </summary>
        /// <param name="text">Text to search.</param>
        public async Task SearchInDialog(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            SearchBoxClearVisibility = true;
            MessagesInSearch.Clear();

            // Perform a search and get the result.
            DialogSearchResult = await ControllerBase.SearchController.SearchInDialog(text, SelectedDialog.Id);
            MessagesNotFoundVisibility = DialogSearchResult.Messages.Length == 0;
            foreach (var dialog in DialogSearchResult.Messages)
            {
                MessagesInSearch.Add(dialog);
            }
        }

        /// <summary>
        /// Validate selected dialog.
        /// </summary>
        private void ValidateSelectedDialog()
        {
            // If the selected dialog is null, hide the input field.
            if (SelectedDialog == null!)
            {
                DialogInputVisibility = false;
                return;
            }

            // If the user in the selected dialog is blocked, hide the input field.
            if (SelectedDialog.User.IsBlocked)
            {
                DialogInputVisibility = false;
            }
            // If I am blocked by the user in the selected dialog, hide the input field.
            else if (SelectedDialog.IsMeBlocked)
            {
                DialogInputVisibility = false;
            }
            else
            {
                DialogInputVisibility = true;
            }
        }

        /// <summary>
        /// Compare the dates of the messages.
        /// </summary>
        /// <param name="firstMessage">First message.</param>
        /// <param name="secondMessage">Second message.</param>
        /// <param name="minMinutes">Minimum minutes between messages.</param>
        /// <returns>True if the dates are different, otherwise false.</returns>
        private bool IsDateDifferent(MessageModel firstMessage, MessageModel secondMessage, int? minMinutes = null)
        {
            // Parse the date and time of the messages.
            var firstSentAt = DateTimeOffset.Parse(firstMessage.SentAt).Date;
            var secondSentAt = DateTimeOffset.Parse(secondMessage.SentAt).Date;

            if (minMinutes == null) return firstSentAt != secondSentAt;

            var firstSentAtTime = DateTimeOffset.Parse(firstMessage.SentAt).TimeOfDay;
            var secondSentAtTime = DateTimeOffset.Parse(secondMessage.SentAt).TimeOfDay;

            // Compare the dates and times of the messages.
            return firstSentAt != secondSentAt || firstSentAtTime - secondSentAtTime > TimeSpan.FromMinutes(minMinutes.Value);
        }

        /// <summary>
        /// Validate messages.
        /// </summary>
        /// <param name="messages">Messages to validate.</param>
        /// <param name="isScrolling">Boolean value indicating whether the dialog are being scrolled.</param>
        private void ValidateMessages(IReadOnlyList<MessageModel> messages, bool isScrolling = false)
        {
            if (messages.Count == 0) return;

            // Loop through the messages.
            for (var i = 0; i < messages.Count; i++)
            {
                var message = messages[i];

                // If the message is the first in the list, set the NewDate property to true.
                if (i == 0)
                {
                    message.NewDate = true;
                    SetPhotoVisibility(message);
                }

                // Get the previous message.
                var pastMessage = i == 0 ? null : messages[i - 1];
                if (pastMessage == null)
                {
                    if (isScrolling)
                        SelectedDialog.Messages.Insert(i, message);
                    continue;
                }

                // Set the NewDate property to true if the date of the current message is different from the date of the previous message.
                message.NewDate = IsDateDifferent(message, pastMessage);

                // Set the IsMyMessage property to true if the current message is sent by the current logged in user.
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

                // Check if the date of the current message is different from the date of the previous message by 10 minutes minimum.
                if (IsDateDifferent(message, pastMessage, 10))
                {
                    SetPhotoVisibility(pastMessage);
                }

                // Lazy load the messages.
                if (!isScrolling) continue;
                SelectedDialog.Messages.Insert(i, message);
            }
        }

        /// <summary>
        /// Validate message.
        /// </summary>
        /// <param name="message">Message to validate.</param>
        /// <param name="dialog">Dialog where the message is located.</param>
        private void ValidateMessage(MessageModel message, DialogModel dialog)
        {
            // Check if no messages in the dialog.
            if (dialog.Messages is { Count: > 0 })
            {
                // Get the last message in the dialog.
                var lastMessage = dialog.Messages.Last();

                message.NewDate = IsDateDifferent(message, lastMessage);

                if (IsDateDifferent(message, lastMessage, 10))
                {
                    SetPhotoVisibility(lastMessage);
                }

                message.IsMyMessage = message.Sender.Id == MyProfile.Id;

                // Compare the sender of the last message and the current message.
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
                dialog.Messages.Add(message);
                window.MessagesListView.Items.Refresh();

                // Scroll to the last message.
                if (SelectedDialog != null! && SelectedDialog.Id == dialog.Id)
                    if (window.MessagesScrollViewer.VerticalOffset > window.MessagesScrollViewer.ExtentHeight -
                        window.MessagesScrollViewer.ViewportHeight - 320 || message.IsMyMessage == true)
                        window.MessagesListView.ScrollIntoView(SelectedDialog.Messages.Last());

                dialog.LastMessage = message;

                // Check if we not read the message yet.
                if (message.IsRead || message.Sender.Id == MyProfile.Id) return;
                dialog.UnreadMessages++;
                UnreadMessagesCount = Dialogs.Count(x => x.UnreadMessages != 0);
            }
            else
            {
                if (Application.Current.MainWindow is not MainWindow window) return;
                message.NewDate = true;
                SetPhotoVisibility(message);
                if (SelectedDialog != null! && SelectedDialog.Id == dialog.Id)
                    SelectedDialog = dialog;
                else
                    dialog.Messages.Add(message);
                window.MessagesListView.Items.Refresh();
                dialog.LastMessage = message;
            }
        }

        /// <summary>
        /// Set user photo to the side of the message
        /// </summary>
        /// <param name="message">Message where the photo will be set.</param>
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

        /// <summary>
        /// Get messages from the server and validate them.
        /// </summary>
        /// <param name="message">Message to validate.</param>
        /// <param name="dialogId">Dialog id where the message is located.</param>
        /// <param name="dialog">Dynamic dialog object.</param>
        public void GetMessage(dynamic message, string dialogId, dynamic dialog)
        {
            // Deserialize the message.
            var newMessage = JsonConvert.DeserializeObject<MessageModel>(message.ToString());

            // Get the dialog.
            var existDialog = Dialogs.FirstOrDefault(x => x.Id == dialogId);
            if (Application.Current.MainWindow is not MainWindow window) return;

            // Check if the dialog exists.
            if (existDialog == null)
            {
                // Deserialize the dialog.
                var newDialog = JsonConvert.DeserializeObject<DialogModel>(dialog.ToString());
                Dialogs.Add(newDialog);
                UnreadMessagesCount = Dialogs.Count(x => x.UnreadMessages != 0);

                // Order dialogs firstly by isPinned, then by lastMessage sentAt
                window.ChatBoxListView.Items.SortDescriptions.Clear();
                window.ChatBoxListView.Items.SortDescriptions.Add(new SortDescription("IsPinned", ListSortDirection.Descending));
                window.ChatBoxListView.Items.SortDescriptions.Add(
                    new SortDescription("LastMessage.SentAt", ListSortDirection.Ascending)
                );

                // Validate the message.
                ValidateMessage(newMessage, newDialog);

                // Reselect the dialog.
                var index = Dialogs.IndexOf(newDialog);
                SelectedDialog = Dialogs[index];
                NoMessagesVisibility = false;
                return;
            }

            // Clear old sort descriptions and add new ones
            window.ChatBoxListView.Items.SortDescriptions.Clear();
            window.ChatBoxListView.Items.SortDescriptions.Add(new SortDescription("IsPinned", ListSortDirection.Descending));
            window.ChatBoxListView.Items.SortDescriptions.Add(
                new SortDescription("LastMessage.SentAt", ListSortDirection.Ascending)
            );

            // Validate the message.
            ValidateMessage(newMessage, existDialog);
        }

        /// <summary>
        /// Read the message.
        /// </summary>
        /// <param name="messageId">Message id.</param>
        /// <param name="dialogId">Dialog id.</param>
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

        /// <summary>
        /// Load more messages from the server.
        /// </summary>
        public async void LoadMoreItems()
        {
            var dialog = SelectedDialog;

            // Check if the current dialog object is different from the previous one before LoadMoreItems was called
            if (dialog != _previousDialog)
            {
                _allItemsLoaded = false;
                _isFetching = false;
                _isLastTime = false;
                _previousDialog = dialog;
            }

            // Check if all items are loaded or if the items are currently loading
            if (_allItemsLoaded || _isFetching) return;

            // Show loading animation
            MessagesLoadingVisibility = true;

            // Set fetching flag to true
            _isFetching = true;
            
            // Object to store new messages
            var newMessages = await ControllerBase.DialogController.LoadMoreMessages(
                dialog.Id,
                dialog.Messages.Count
            );

            // Check if the newMessages count is less than 100
            // Default count of messages to load is 100
            if (newMessages.Count != 100)
            {
                _allItemsLoaded = true;
                MessagesLoadingVisibility = false;
            }

            // Check if the newMessages count is 0
            if (_isLastTime)
            {
                _isFetching = false;
                return;
            }

            // Validate the messages
            ValidateMessages(newMessages, true);

            // Update the layout
            if (Application.Current.MainWindow is not MainWindow window) return;
            window.MessagesListView.Items.Refresh();

            window.MessagesListView.ScrollIntoView(newMessages.Last());

            // Check if all items are loaded
            if (_allItemsLoaded)
            {
                _isLastTime = true;
                MessagesLoadingVisibility = false;
            }
            MessagesLoadingVisibility = false;
            _isFetching = false;
        }

        /// <summary>
        /// Main view model.
        /// </summary>
        public MainViewModel()
        {
            // Set current page to default page.
            CurrentPage = SettingsManager.DefaultPage;

            // Get my profile.
            MyProfile = ControllerBase.UserController.GetMyProfile();

            // Get my sessions.
            var sessions = ControllerBase.UserController.GetMySessions() ?? null;
            Sessions = sessions != null ? new ObservableCollection<SessionsModel?>(sessions) : null;

            // Check if sessions is not null.
            if (MyProfile.Sessions != null)
            {
                SessionsCount = MyProfile.Sessions.Count;
                // Get current session
                CurrentSession = Sessions?.FirstOrDefault(x => x?.Current == true);
                // Remove current session from Sessions
                if (CurrentSession != null)
                {
                    Sessions?.Remove(CurrentSession);
                }
            }
            MyProfile.Sessions = Sessions;

            // Get my black list.
            BlockedUsersCount = MyProfile.BlackList.Count;

            // Get my dialogs.
            var dialogs = ControllerBase.DialogController.GetDialogs() ?? null;

            // Check if dialogs is not null.
            if (dialogs != null)
            {
                // Check if dialogs is not empty.
                if (dialogs.Length != 0)
                {
                    // Add dialogs to the Dialogs collection.
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

                    // Validate all messages in the dialogs.
                    foreach (var dialog in Dialogs)
                    {
                        ValidateMessages(dialog.Messages);
                    }
                }
            }
            else
            {
                NoMessagesVisibility = true;
                UnreadMessagesCount = 0;
            }

            // Switch method for language.
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

            if (MyProfile.Settings != null)
                // Switch method for last activity status.
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

            // Clear the search box command.
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

            // Clear the search input box method.
            void ResetSearchFields(MainWindow window)
            {
                window.SearchBox.Text = string.Empty;
                window.SearchBoxSecond.Text = string.Empty;
            }

            // Toggle search control visibility method.
            void ToggleSearchControlVisibility(MainWindow window, bool isSearching)
            {
                // Checkers
                window.SearchAllPanel.Visibility = isSearching ? Visibility.Visible : Visibility.Collapsed;
                window.SearchInChatPanel.Visibility = Visibility.Collapsed;
                window.DialogsPanel.Visibility = isSearching ? Visibility.Collapsed : Visibility.Visible;
                window.ClearSearchBoxButton.Visibility = isSearching ? Visibility.Visible : Visibility.Collapsed;
                window.SearchBoxSecond.Visibility = isSearching ? Visibility.Visible : Visibility.Collapsed;
                window.SearchBox.Visibility = isSearching ? Visibility.Collapsed : Visibility.Visible;
            }

            // Clear collections method.
            void ClearCollections()
            {
                DialogsInSearch.Clear();
                UsersInSearch.Clear();
                MessagesInSearch.Clear();
            }
        }
    }
}
