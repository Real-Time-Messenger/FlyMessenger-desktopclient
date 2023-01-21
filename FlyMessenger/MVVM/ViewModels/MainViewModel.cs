using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FlyMessenger.Controllers;
using FlyMessenger.Core;
using FlyMessenger.MVVM.Model;
using FlyMessenger.Properties;
using FlyMessenger.Resources.Languages;
using FlyMessenger.Resources.Settings;

namespace FlyMessenger.MVVM.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public static ObservableCollection<DialogModel>? Dialogs { get; private set; }
        public static ObservableCollection<SessionsModel>? Sessions { get; private set; }
        public RelayCommand SendCommand { get; set; }
        public RelayCommand ClearSearchBox { get; set; }

        private string _searchBoxText;

        public string SearchBoxText
        {
            get => _searchBoxText;
            set
            {
                _searchBoxText = value;
                OnPropertyChanged();
                Search();
            }
        }

        // TODO: Implement Search functionality
        private async void Search()
        {
            if (string.IsNullOrEmpty(SearchBoxText))
                return;

            SearchResult = await ControllerBase.SearchController.Search(SearchBoxText);

            SearchBoxClearVisibility = !string.IsNullOrEmpty(SearchBoxText);
        }

        private DialogModel _selectedDialog;

        public DialogModel SelectedDialog
        {
            get => _selectedDialog;
            set
            {
                _selectedDialog = value;
                OnPropertyChanged();
            }
        }

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

        private int _unreadedMessagesCount;

        public int UnreadedMessagesCount
        {
            get => _unreadedMessagesCount;
            set
            {
                _unreadedMessagesCount = value;
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

        private bool _canGoBack;

        public bool CanGoBack
        {
            get => _canGoBack;
            set
            {
                _canGoBack = value;
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

        public MainViewModel()
        {
            CurrentPage = SettingsManager.DefaultPage;

            // Get all dialogs
            if (ControllerBase.DialogController.GetDialogs() is { } dialogs)
            {
                Dialogs = new ObservableCollection<DialogModel>(dialogs);
                NoMessagesVisibility = false;

                // Sort by MessageDateTime and MessageAnchor
                Dialogs = new ObservableCollection<DialogModel>(
                    Dialogs.OrderByDescending(x => x.IsPinned).ThenByDescending(x => x.LastMessage?.SentAt)
                );

                // Count how many MessagesCount are
                UnreadedMessagesCount = Dialogs.Count(x => x.UnreadMessages != 0);
            }
            else
            {
                NoMessagesVisibility = true;
                UnreadedMessagesCount = 0;
            }

            MyProfile = ControllerBase.UserController.GetMyProfile();
            Sessions = new ObservableCollection<SessionsModel>(ControllerBase.UserController.GetMySessions());
            SessionsCount = MyProfile.Sessions.Length;
            BlockedUsersCount = MyProfile.BlackList.Length;

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

            ClearSearchBox = new RelayCommand(
                _ =>
                {
                    SearchBoxText = string.Empty;
                    OnPropertyChanged(nameof(SearchBoxText));
                },
                _ => true
            );
        }
    }
}
