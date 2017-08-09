using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xamarin.Forms;

using XamList.Shared;

namespace XamList
{
    public class ContactsListViewModel : BaseViewModel
    {
        #region Fields
        bool _isRefreshCommandExecuting;
        ICommand _refreshCommand, _restoreDeletedContactsCommand;
        IList<ContactModel> _allContactsList;
        #endregion

        #region Events
        public event EventHandler PullToRefreshCompleted;
        public event EventHandler RestoreDeletedContactsCompleted;
        #endregion

        #region Properties
        public ICommand RefreshCommand => _refreshCommand ??
            (_refreshCommand = new Command(async () =>
            {
                MobileCenterHelpers.TrackEvent(MobileCenterConstants.PullToRefreshTriggered);
                await ExecuteRefreshCommand();
            }));

        public ICommand RestoreDeletedContactsCommand => _restoreDeletedContactsCommand ??
            (_restoreDeletedContactsCommand = new Command(async () =>
            {
                MobileCenterHelpers.TrackEvent(MobileCenterConstants.RestoreDeletedContactsTapped);
                await ExecuteRestoreDeletedContactsCommand();
            }));

        public IList<ContactModel> AllContactsList
        {
            get => _allContactsList;
            set => SetProperty(ref _allContactsList, value);
        }
        #endregion

        #region Methods
        async Task ExecuteRefreshCommand()
        {
            if (_isRefreshCommandExecuting)
                return;

            _isRefreshCommandExecuting = true;
            try
            {
                var oneSecondTaskToShowSpinner = Task.Delay(1000);

                await DatabaseSyncService.SyncRemoteAndLocalDatabases().ConfigureAwait(false);

                var contactList = await ContactDatabase.GetAllContacts().ConfigureAwait(false);
                AllContactsList = contactList.Where(x => !x.IsDeleted).OrderBy(x => x.FullName).ToList();

                await oneSecondTaskToShowSpinner;
            }
            catch (Exception e)
            {
                MobileCenterHelpers.Log(e);
            }
            finally
            {
                OnPullToRefreshCompleted();
                _isRefreshCommandExecuting = false;
            }
        }

        async Task ExecuteRestoreDeletedContactsCommand()
        {
            await APIService.RestoreDeletedContacts().ConfigureAwait(false);
            OnRestoreDeletedContactsCompleted();
        }

        void OnPullToRefreshCompleted() =>
            PullToRefreshCompleted?.Invoke(this, EventArgs.Empty);

        void OnRestoreDeletedContactsCompleted() =>
            RestoreDeletedContactsCompleted?.Invoke(this, EventArgs.Empty);
        #endregion
    }
}
