using System;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;

using AsyncAwaitBestPractices.MVVM;

using XamList.Shared;
using XamList.Mobile.Shared;

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
            (_refreshCommand = new AsyncCommand(() =>
            {
                AppCenterHelpers.TrackEvent(AppCenterConstants.PullToRefreshTriggered);
                return ExecuteRefreshCommand();
            }));
        
        public ICommand RestoreDeletedContactsCommand => _restoreDeletedContactsCommand ??
            (_restoreDeletedContactsCommand = new AsyncCommand(() =>
            {
                AppCenterHelpers.TrackEvent(AppCenterConstants.RestoreDeletedContactsTapped);
                return ExecuteRestoreDeletedContactsCommand();
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
                await ApiService.RestoreDeletedContacts().ConfigureAwait(false);

                await ExecuteRefreshCommand().ConfigureAwait(false);

                await minimumSpinnerTime.ConfigureAwait(false);
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        #endregion
    }
}
