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
                await SyncRemoteAndLocalDatabases().ConfigureAwait(false);

                var contactList = await ContactDatabase.GetAllContacts().ConfigureAwait(false);
                AllContactsList = contactList.Where(x => !x.IsDeleted).OrderBy(x => x.FullName).ToList();
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

        async Task SyncRemoteAndLocalDatabases()
        {
            var (contactListFromLocalDatabase, contactListFromRemoteDatabase) = await GetAllSavedContacts();

            var (contactsInLocalDatabaseButNotStoredRemotely, contactsInRemoteDatabaseButNotStoredLocally, contactsInBothDatabases) = GetMatchingContacts(contactListFromLocalDatabase, contactListFromRemoteDatabase);

            var (contactsToPatchToLocalDatabase, contactsToPatchToRemoteDatabase) = GetContactListsThatNeedUpdating(contactListFromRemoteDatabase, contactListFromRemoteDatabase, contactsInBothDatabases);

            await SaveContacts(contactsToPatchToRemoteDatabase,
                                      contactsInRemoteDatabaseButNotStoredLocally.Concat(contactsToPatchToLocalDatabase).ToList(),
                                      contactsInLocalDatabaseButNotStoredRemotely);
        }

        (List<ContactModel> contactsToPatchToLocalDatabase, 
         List<ContactModel> contactsToPatchToRemoteDatabase) GetContactListsThatNeedUpdating(List<ContactModel> contactListFromLocalDatabase, List<ContactModel> contactListFromRemoteDatabase, List<ContactModel> contactsInBothDatabases)
        {
            var contactsToPatchToRemoteDatabase = new List<ContactModel>();
            var contactsToPatchToLocalDatabase = new List<ContactModel>();
            foreach (var contact in contactsInBothDatabases)
            {
                var contactFromLocalDatabase = contactListFromLocalDatabase.Where(x => x.Id.Equals(contact.Id)).FirstOrDefault();
                var contactFromRemoteDatabase = contactListFromRemoteDatabase.Where(x => x.Id.Equals(contact.Id)).FirstOrDefault();

                if (contactFromLocalDatabase?.UpdatedAt.CompareTo(contactFromRemoteDatabase?.UpdatedAt ?? default(DateTimeOffset)) > 0)
                    contactsToPatchToRemoteDatabase.Add(contact);
                else if (contactFromLocalDatabase?.UpdatedAt.CompareTo(contactFromRemoteDatabase?.UpdatedAt ?? default(DateTimeOffset)) < 0)
                    contactsToPatchToLocalDatabase.Add(contact);
            }

            return (contactsToPatchToLocalDatabase ?? new List<ContactModel>(), 
                    contactsToPatchToRemoteDatabase ?? new List<ContactModel>());
        }

        (List<ContactModel> contactsInLocalDatabaseButNotStoredRemotely,
         List<ContactModel> contactsInRemoteDatabaseButNotStoredLocally,
         List<ContactModel> contactsInBothDatabases) GetMatchingContacts(List<ContactModel> contactListFromLocalDatabase, List<ContactModel> contactListFromRemoteDatabase)
        {
            var contactIdFromRemoteDatabaseList = contactListFromRemoteDatabase?.Select(x => x.Id).ToList() ?? new List<string>();
            var contactIdFromLocalDatabaseList = contactListFromLocalDatabase?.Select(x => x.Id).ToList() ?? new List<string>();

            var contactIdsInRemoteDatabaseButNotStoredLocally = contactIdFromRemoteDatabaseList?.Except(contactIdFromLocalDatabaseList)?.ToList() ?? new List<string>();
            var contactIdsInLocalDatabaseButNotStoredRemotely = contactIdFromLocalDatabaseList?.Except(contactIdFromRemoteDatabaseList)?.ToList() ?? new List<string>();
            var contactIdsInBothDatabases = contactIdFromRemoteDatabaseList?.Where(x => contactIdFromLocalDatabaseList?.Contains(x) ?? false).ToList() ?? new List<string>();

            var contactsInRemoteDatabaseButNotStoredLocally = contactListFromRemoteDatabase?.Where(x => contactIdsInRemoteDatabaseButNotStoredLocally?.Contains(x?.Id) ?? false).ToList() ?? new List<ContactModel>();
            var contactsInLocalDatabaseButNotStoredRemotely = contactListFromLocalDatabase?.Where(x => contactIdsInLocalDatabaseButNotStoredRemotely?.Contains(x?.Id) ?? false).ToList() ?? new List<ContactModel>();

            var contactsInBothDatabases = contactListFromLocalDatabase?.Where(x => contactIdsInBothDatabases?.Contains(x?.Id) ?? false)
                                            .Concat(contactListFromRemoteDatabase?.Where(x => contactIdsInBothDatabases?.Contains(x?.Id) ?? false)).ToList() ?? new List<ContactModel>();

            return (contactsInLocalDatabaseButNotStoredRemotely ?? new List<ContactModel>(), 
                    contactsInRemoteDatabaseButNotStoredLocally ?? new List<ContactModel>(), 
                    contactsInBothDatabases ?? new List<ContactModel>());

        }

        async Task<(List<ContactModel> contactListFromLocalDatabase, 
                    List<ContactModel> contactListFromRemoteDatabase)> GetAllSavedContacts()
        {
            var contactListFromLocalDatabaseTask = ContactDatabase.GetAllContacts();
            var contactListFromRemoteDatabaseTask = APIService.GetAllContactModels();

            await Task.WhenAll(contactListFromLocalDatabaseTask, contactListFromRemoteDatabaseTask).ConfigureAwait(false);

            return (contactListFromLocalDatabaseTask.Result ?? new List<ContactModel>(), 
                    contactListFromRemoteDatabaseTask.Result ?? new List<ContactModel>());
        }

        async Task SaveContacts(List<ContactModel> contactsToPatchToRemoteDatabase, 
                                List<ContactModel> contactsToSaveToLocalDatabase, 
                                List<ContactModel> contactsToPostToRemoteDatabase)
        {
            var saveContactTaskList = new List<Task>();
            foreach (var contact in contactsToPostToRemoteDatabase)
                saveContactTaskList.Add(APIService.PostContactModel(contact));

            foreach (var contact in contactsToSaveToLocalDatabase)
                saveContactTaskList.Add(ContactDatabase.SaveContact(contact));

            foreach (var contact in contactsToPatchToRemoteDatabase)
                saveContactTaskList.Add(APIService.PatchContactModel(contact));

            await Task.WhenAll(saveContactTaskList).ConfigureAwait(false);
        }

        async Task ExecuteRestoreDeletedContactsCommand()
        {
            throw new Exception("ExecuteRestoreDeletedContactsCommand Not Implemented");
        }

        void OnPullToRefreshCompleted() =>
            PullToRefreshCompleted?.Invoke(this, EventArgs.Empty);
        #endregion
    }
}
