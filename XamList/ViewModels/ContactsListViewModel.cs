using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using XamList.Mobile.Shared;
using XamList.Shared;

namespace XamList
{
    public class ContactsListViewModel : BaseViewModel
    {
        bool _isRefreshing;
        ICommand? _refreshCommand, _restoreDeletedContactsCommand;

        public ICommand RefreshCommand => _refreshCommand ??= new AsyncCommand(() =>
        {
            AppCenterHelpers.TrackEvent(AppCenterConstants.PullToRefreshTriggered);
            return ExecuteRefreshCommand();
        });

        public ICommand RestoreDeletedContactsCommand => _restoreDeletedContactsCommand ??= new AsyncCommand(() =>
        {
            AppCenterHelpers.TrackEvent(AppCenterConstants.RestoreDeletedContactsTapped);
            return ExecuteRestoreDeletedContactsCommand();
        });

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public ObservableCollection<ContactModel> AllContactsList { get; } = new ObservableCollection<ContactModel>();

        async Task ExecuteRefreshCommand()
        {
            AllContactsList.Clear();

            try
            {
                var minimumSpinnerTime = Task.Delay(1000);

                await DatabaseSyncService.SyncRemoteAndLocalDatabases().ConfigureAwait(false);

                var contactList = await ContactDatabase.GetAllContacts().ConfigureAwait(false);

                foreach (var contact in contactList.Where(x => !string.IsNullOrWhiteSpace(x?.FullName) && !x.IsDeleted).OrderBy(x => x.FullName))
                {
                    AllContactsList.Add(contact);
                }

                await minimumSpinnerTime.ConfigureAwait(false);
            }
            catch (Exception e)
            {
                AppCenterHelpers.Report(e);
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        async Task ExecuteRestoreDeletedContactsCommand()
        {
            IsRefreshing = true;

            var minimumSpinnerTime = Task.Delay(5000);

            try
            {
                await ApiService.RestoreDeletedContacts().ConfigureAwait(false);

                await ExecuteRefreshCommand().ConfigureAwait(false);

                await minimumSpinnerTime.ConfigureAwait(false);
            }
            finally
            {
                IsRefreshing = false;
            }
        }
    }
}
