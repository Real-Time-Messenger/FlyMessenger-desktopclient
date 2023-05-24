using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FlyMessenger.MVVM.Model
{
    /// <summary>
    /// Model for last search history.
    /// </summary>
    public class LastItemModel
    {
        public ObservableCollection<DialogModel> Dialogs { get; set; } = new ObservableCollection<DialogModel>();
        public ObservableCollection<DialogModel> Messages { get; set; } = new ObservableCollection<DialogModel>();
        public ObservableCollection<DialogModel> Users { get; set; } = new ObservableCollection<DialogModel>();
    }

    /// <summary>
    /// Limit size of last search history.
    /// </summary>
    public static class LimitedSizeStack
    {
        public static void Push(DialogModel item, bool isDialog, bool isUser)
        {
            if (isDialog)
                MainWindow.MainViewModel.LastItemsInSearch.Dialogs.Insert(0, item);
            else if (isUser)
                MainWindow.MainViewModel.LastItemsInSearch.Users.Insert(0, item);
            else
                MainWindow.MainViewModel.LastItemsInSearch.Messages.Insert(0, item);

            // Remove last collection item if size > 3
            if ((!isDialog || MainWindow.MainViewModel.LastItemsInSearch.Dialogs.Count <= 3) &&
                (!isUser || MainWindow.MainViewModel.LastItemsInSearch.Users.Count <= 3) &&
                (isDialog || isUser || MainWindow.MainViewModel.LastItemsInSearch.Messages.Count <= 3)) return;
            
            if (isDialog)
                MainWindow.MainViewModel.LastItemsInSearch.Dialogs.Remove(
                    MainWindow.MainViewModel.LastItemsInSearch.Dialogs.Last()
                );
            else if (isUser)
                MainWindow.MainViewModel.LastItemsInSearch.Users.Remove(
                    MainWindow.MainViewModel.LastItemsInSearch.Users.Last()
                );
            else
                MainWindow.MainViewModel.LastItemsInSearch.Messages.Remove(
                    MainWindow.MainViewModel.LastItemsInSearch.Messages.Last()
                );
        }
    }
}
