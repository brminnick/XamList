using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using XamList.Shared;
using XamList.Mobile.Shared;

namespace XamList
{
    public class DatabaseSyncService
    {
        readonly ApiService _apiService;
        readonly ContactDatabase _contactDatabase;

        public DatabaseSyncService(ContactDatabase contactDatabase, ApiService apiService) =>
            (_contactDatabase, _apiService) = (contactDatabase, apiService);

        public async Task SyncRemoteAndLocalDatabases()
        {
            var (contactListFromLocalDatabase, contactListFromRemoteDatabase) = await GetAllSavedContacts().ConfigureAwait(false);

            var (contactsInLocalDatabaseButNotStoredRemotely, contactsInRemoteDatabaseButNotStoredLocally, contactsInBothDatabases) = GetMatchingModels(contactListFromLocalDatabase, contactListFromRemoteDatabase);

            var (contactsToPatchToLocalDatabase, contactsToPatchToRemoteDatabase) = GetModelsThatNeedUpdating(contactListFromLocalDatabase, contactListFromRemoteDatabase, contactsInBothDatabases);

            await SaveContacts(contactsToPatchToRemoteDatabase,
                                contactsToPatchToLocalDatabase,
                                contactsInRemoteDatabaseButNotStoredLocally,
                                contactsInLocalDatabaseButNotStoredRemotely).ConfigureAwait(false);
        }

        async Task<(IEnumerable<ContactModel> contactListFromLocalDatabase,
           IEnumerable<ContactModel> contactListFromRemoteDatabase)> GetAllSavedContacts()
        {
            var contactListFromLocalDatabaseTask = _contactDatabase.GetAllContacts();
            var contactListFromRemoteDatabaseTask = _apiService.GetAllContactModels();

            await Task.WhenAll(contactListFromLocalDatabaseTask, contactListFromRemoteDatabaseTask).ConfigureAwait(false);

            return (await contactListFromLocalDatabaseTask.ConfigureAwait(false), await contactListFromRemoteDatabaseTask.ConfigureAwait(false));

        }

        (IEnumerable<T> contactsInLocalDatabaseButNotStoredRemotely,
           IEnumerable<T> contactsInRemoteDatabaseButNotStoredLocally,
           IEnumerable<T> contactsInBothDatabases) GetMatchingModels<T>(IEnumerable<T> modelListFromLocalDatabase,
                                                                     IEnumerable<T> modelListFromRemoteDatabase) where T : IBaseModel
        {
            var modelIdFromRemoteDatabaseList = modelListFromRemoteDatabase.Select(x => x.Id);
            var modelIdFromLocalDatabaseList = modelListFromLocalDatabase.Select(x => x.Id);

            var modelIdsInRemoteDatabaseButNotStoredLocally = modelIdFromRemoteDatabaseList.Except(modelIdFromLocalDatabaseList);
            var modelIdsInLocalDatabaseButNotStoredRemotely = modelIdFromLocalDatabaseList.Except(modelIdFromRemoteDatabaseList);
            var modelIdsInBothDatabases = modelIdFromRemoteDatabaseList.Where(x => modelIdFromLocalDatabaseList.Contains(x));

            var modelsInRemoteDatabaseButNotStoredLocally = modelListFromRemoteDatabase.Where(x => modelIdsInRemoteDatabaseButNotStoredLocally.Contains(x.Id));
            var modelsInLocalDatabaseButNotStoredRemotely = modelListFromLocalDatabase.Where(x => modelIdsInLocalDatabaseButNotStoredRemotely.Contains(x.Id));

            var modelsInBothDatabases = modelListFromLocalDatabase.Where(x => modelIdsInBothDatabases.Contains(x.Id));

            return (modelsInLocalDatabaseButNotStoredRemotely, modelsInRemoteDatabaseButNotStoredLocally, modelsInBothDatabases);
        }

        (IEnumerable<T> contactsToPatchToLocalDatabase,
               IEnumerable<T> contactsToPatchToRemoteDatabase) GetModelsThatNeedUpdating<T>(IEnumerable<T> modelListFromLocalDatabase,
                                                                             IEnumerable<T> modelListFromRemoteDatabase,
                                                                             IEnumerable<T> modelsFoundInBothDatabases) where T : IBaseModel
        {
            var modelsToPatchToRemoteDatabase = new List<T>();
            var modelsToPatchToLocalDatabase = new List<T>();

            foreach (var contact in modelsFoundInBothDatabases)
            {
                var modelFromLocalDatabase = modelListFromLocalDatabase.Where(x => x.Id.Equals(contact.Id)).FirstOrDefault();
                var modelFromRemoteDatabase = modelListFromRemoteDatabase.Where(x => x.Id.Equals(contact.Id)).FirstOrDefault();

                if (modelFromLocalDatabase?.UpdatedAt.CompareTo(modelFromRemoteDatabase?.UpdatedAt ?? default) > 0)
                {
                    modelsToPatchToRemoteDatabase.Add(modelFromLocalDatabase);
                }
                else if (modelFromRemoteDatabase is not null
                            && modelFromLocalDatabase?.UpdatedAt.CompareTo(modelFromRemoteDatabase.UpdatedAt) < 0)
                {
                    modelsToPatchToLocalDatabase.Add(modelFromRemoteDatabase);
                }
            }

            return (modelsToPatchToLocalDatabase, modelsToPatchToRemoteDatabase);
        }

        Task SaveContacts(IEnumerable<ContactModel> contactsToPatchToRemoteDatabase,
                                   IEnumerable<ContactModel> contactsToPatchToLocalDatabase,
                                   IEnumerable<ContactModel> contactsToAddToLocalDatabase,
                                   IEnumerable<ContactModel> contactsToPostToRemoteDatabase)
        {
            var saveContactTaskList = new List<Task>();
            foreach (var contact in contactsToPostToRemoteDatabase)
                saveContactTaskList.Add(_apiService.PostContactModel(contact));

            foreach (var contact in contactsToAddToLocalDatabase)
                saveContactTaskList.Add(_contactDatabase.SaveContact(contact));

            foreach (var contact in contactsToPatchToRemoteDatabase)
                saveContactTaskList.Add(_apiService.PatchContactModel(contact));

            foreach (var contact in contactsToPatchToLocalDatabase)
                saveContactTaskList.Add(_contactDatabase.PatchContactModel(contact));

            return Task.WhenAll(saveContactTaskList);
        }
    }
}
