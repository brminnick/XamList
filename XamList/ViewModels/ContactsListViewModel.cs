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
        readonly ApiService _apiService;
        readonly ContactDatabase _contactDatabase;
        readonly DatabaseSyncService _databaseSyncService;

        bool _isRefreshing;

        public ContactsListViewModel(ApiService apiService,
                                        ContactDatabase contactDatabase,
                                        AppCenterService appCenterService,
                                        DatabaseSyncService databaseSyncService) : base(appCenterService)
        {
            _apiService = apiService;
            _contactDatabase = contactDatabase;
            _databaseSyncService = databaseSyncService;

            RefreshCommand = new AsyncCommand(() =>
            {
                AppCenterService.Track(AppCenterConstants.PullToRefreshTriggered);
                return ExecuteRefreshCommand();
            });

            RestoreDeletedContactsCommand = new AsyncCommand(() =>
            {
                AppCenterService.Track(AppCenterConstants.RestoreDeletedContactsTapped);
                return ExecuteRestoreDeletedContactsCommand();
            });
        }

        public ICommand RefreshCommand { get; }
        public ICommand RestoreDeletedContactsCommand { get; }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public ObservableCollection<ContactModel> AllContactsList { get; } = new ObservableCollection<ContactModel>();

        async Task ExecuteRefreshCommand()
        {
            var minimumSpinnerTime = Task.Delay(1000);

            AllContactsList.Clear();

            try
            {

                await _databaseSyncService.SyncRemoteAndLocalDatabases().ConfigureAwait(false);

                var contactList = await _contactDatabase.GetAllContacts().ConfigureAwait(false);

                foreach (var contact in contactList.Where(x => !string.IsNullOrWhiteSpace(x.FullName) && !x.IsDeleted).OrderBy(x => x.FullName))
                {
                    AllContactsList.Add(contact);
                }

            }
            catch (Exception e)
            {
                AppCenterService.Report(e);
            }
            finally
            {
                await minimumSpinnerTime.ConfigureAwait(false);
                IsRefreshing = false;
            }
        }

        async Task ExecuteRestoreDeletedContactsCommand()
        {
            IsRefreshing = true;

            var minimumSpinnerTime = Task.Delay(5000);

            try
            {
                await _apiService.RestoreDeletedContacts().ConfigureAwait(false);

                await ExecuteRefreshCommand().ConfigureAwait(false);
            }
            finally
            {
                await minimumSpinnerTime.ConfigureAwait(false);
                IsRefreshing = false;
            }
        }
    }
}
