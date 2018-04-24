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
        bool _isRefreshing;
        ICommand _refreshCommand, _restoreDeletedContactsCommand;
        IList<ContactModel> _allContactsList;
        #endregion

        #region Properties
        public ICommand RefreshCommand => _refreshCommand ??
            (_refreshCommand = new Command(async () =>
            {
                AppCenterHelpers.TrackEvent(MobileCenterConstants.PullToRefreshTriggered);
                await ExecuteRefreshCommand().ConfigureAwait(false);
            }));
        
        public ICommand RestoreDeletedContactsCommand => _restoreDeletedContactsCommand ??
            (_restoreDeletedContactsCommand = new Command(async () =>
            {
                AppCenterHelpers.TrackEvent(MobileCenterConstants.RestoreDeletedContactsTapped);
                await ExecuteRestoreDeletedContactsCommand().ConfigureAwait(false);
            }));

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public IList<ContactModel> AllContactsList
        {
            get => _allContactsList;
            set => SetProperty(ref _allContactsList, value);
        }
        #endregion

        #region Methods
        async Task ExecuteRefreshCommand()
        {
            IsRefreshing = true;

            try
            {
                var minimumSpinnerTime = Task.Delay(1000);

                await DatabaseSyncService.SyncRemoteAndLocalDatabases().ConfigureAwait(false);

                var contactList = await ContactDatabase.GetAllContacts().ConfigureAwait(false);
                AllContactsList = contactList.Where(x => !x.IsDeleted).OrderBy(x => x.FullName).ToList();

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
                await APIService.RestoreDeletedContacts().ConfigureAwait(false);
                await ExecuteRefreshCommand().ConfigureAwait(false);
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        #endregion
    }
}
