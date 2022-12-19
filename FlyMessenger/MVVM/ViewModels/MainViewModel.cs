using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using FlyMessenger.Core;
using FlyMessenger.MVVM.Model;

namespace FlyMessenger.MVVM.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public static ObservableCollection<MessageModel>? Messages { get; private set; }
        public ObservableCollection<ContactModel> Contacts { get; set; }
        public RelayCommand SendCommand { get; set; }
        public RelayCommand ClearSearchBox { get; set; }

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
        private void Search()
        {
            if (string.IsNullOrEmpty(SearchBoxText))
            {
                SearchBoxClearVisibility = false;
            }
            else
            {
                SearchBoxClearVisibility = true;
            }
        }

        private MessageModel _selectedMessage;

        public MessageModel SelectedMessage
        {
            get => _selectedMessage;
            set
            {
                _selectedMessage = value;
                OnPropertyChanged();
            }
        }

        private ContactModel _selectedContact;


        public ContactModel SelectedContact
        {
            get => _selectedContact;
            set
            {
                _selectedContact = value;
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

        private string _message;
        private string _searchBoxText;

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            Messages = new ObservableCollection<MessageModel>
            {
                // Insert some test messages with index, if message is pinned, then it will be shown on top of the list,
                // and if message datetime is newer, then it will be shown on top of the list, but below pinned messages
                new MessageModel
                {
                    MessageAnchor = true,
                    Message = "Hello, this is pinned message i like your mum it's too fat bitch fuck you crazy Kirill",
                    MessageDateTime = "2027-01-01 12:00:00",
                    MessageOwner = "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB",
                    MessagesCount = "234",
                    MessageOwnerImage = "https://picsum.photos/400?random=1",
                },
                new MessageModel
                {
                    MessageAnchor = true,
                    Message = "Hello, this is pinned message",
                    MessageDateTime = "2021-01-01 12:00:00",
                    MessageOwner = "George",
                    MessagesCount = "0",
                    MessageOwnerImage = "https://picsum.photos/400?random=2",
                },
                new MessageModel
                {
                    MessageAnchor = false,
                    Message = "Hello, this is not pinned message",
                    MessageDateTime = "2022-01-01 12:00:00",
                    MessageOwner = "John",
                    MessagesCount = "20",
                    MessageOwnerImage = "https://picsum.photos/400?random=3",
                },
                new MessageModel
                {
                    MessageAnchor = false,
                    Message = "Hello, this is not pinned message",
                    MessageDateTime = "2023-01-01 12:00:00",
                    MessageOwner = "Doe",
                    MessagesCount = "0",
                    MessageOwnerImage = "https://picsum.photos/400?random=4",
                },
                new MessageModel
                {
                    MessageAnchor = false,
                    Message = "Hello, this is not pinned message",
                    MessageDateTime = "2024-01-01 12:00:00",
                    MessageOwner = "Peppa",
                    MessagesCount = "0",
                    MessageOwnerImage = "https://picsum.photos/400?random=5",
                },
                new MessageModel
                {
                    MessageAnchor = false,
                    Message = "Hello, this is not pinned message",
                    MessageDateTime = "2025-01-01 12:00:00",
                    MessageOwner = "George",
                    MessagesCount = "0",
                    MessageOwnerImage = "https://picsum.photos/400?random=6",
                },
                new MessageModel
                {
                    MessageAnchor = false,
                    Message = "Hello, this is not pinned message",
                    MessageDateTime = "2025-01-01 12:00:00",
                    MessageOwner = "George",
                    MessagesCount = "0",
                    MessageOwnerImage = "https://picsum.photos/400?random=7",
                },
                new MessageModel
                {
                    MessageAnchor = false,
                    Message = "Hello, this is not pinned message",
                    MessageDateTime = "2025-01-01 12:00:00",
                    MessageOwner = "George",
                    MessagesCount = "0",
                    MessageOwnerImage = "https://picsum.photos/400?random=8",
                },
                new MessageModel
                {
                    MessageAnchor = false,
                    Message = "Hello, this is not pinned message",
                    MessageDateTime = "2025-01-01 12:00:00",
                    MessageOwner = "George",
                    MessagesCount = "0",
                    MessageOwnerImage = "https://picsum.photos/400?random=9",
                },
                new MessageModel
                {
                    MessageAnchor = false,
                    Message = "Hello, this is not pinned message",
                    MessageDateTime = "2025-01-01 12:00:00",
                    MessageOwner = "George",
                    MessageOwnerImage = "https://picsum.photos/400?random=10",
                },
                new MessageModel
                {
                    MessageAnchor = false,
                    Message = "Hello, this is not pinned message",
                    MessageDateTime = "2025-01-01 12:00:00",
                    MessageOwner = "George",
                    MessagesCount = "0",
                    MessageOwnerImage = "https://picsum.photos/400?random=6",
                }
            };

            // Sort by MessageDateTime and MessageAnchor
            Messages = new ObservableCollection<MessageModel>(
                Messages.OrderByDescending(x => x.MessageAnchor).ThenByDescending(x => x.MessageDateTime)
            );

            // Count how many MessagesCount are
            UnreadedMessagesCount = Messages.Count(x => x.MessagesCount != "0" && x.MessagesCount != null);

            ClearSearchBox = new RelayCommand(
                _ =>
                {
                    SearchBoxText = string.Empty;
                    OnPropertyChanged();
                },
                _ => true
            );
        }
    }
}
