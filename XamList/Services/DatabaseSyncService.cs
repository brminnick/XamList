using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using XamList.Shared;
using XamList.Mobile.Shared;

namespace XamList
{
    public static class DatabaseSyncService
    {
        public static async Task SyncRemoteAndLocalDatabases()
        {
            var (contactListFromLocalDatabase, contactListFromRemoteDatabase) = await GetAllSavedContacts().ConfigureAwait(false);

            var (contactsInLocalDatabaseButNotStoredRemotely, contactsInRemoteDatabaseButNotStoredLocally, contactsInBothDatabases) = GetMatchingModels(contactListFromLocalDatabase, contactListFromRemoteDatabase);

            var (contactsToPatchToLocalDatabase, contactsToPatchToRemoteDatabase) = GetModelsThatNeedUpdating(contactListFromLocalDatabase, contactListFromRemoteDatabase, contactsInBothDatabases);

            await SaveContacts(contactsToPatchToRemoteDatabase,
                                contactsToPatchToLocalDatabase,
                                contactsInRemoteDatabaseButNotStoredLocally,
                                contactsInLocalDatabaseButNotStoredRemotely).ConfigureAwait(false);
        }

        static async Task<(IEnumerable<ContactModel> contactListFromLocalDatabase,
            IEnumerable<ContactModel> contactListFromRemoteDatabase)> GetAllSavedContacts()
        {
            var contactListFromLocalDatabaseTask = ContactDatabase.GetAllContacts();
            var contactListFromRemoteDatabaseTask = ApiService.GetAllContactModels();

            await Task.WhenAll(contactListFromLocalDatabaseTask, contactListFromRemoteDatabaseTask).ConfigureAwait(false);

            return (await contactListFromLocalDatabaseTask.ConfigureAwait(false) ?? Enumerable.Empty<ContactModel>(),
                    await contactListFromRemoteDatabaseTask.ConfigureAwait(false) ?? Enumerable.Empty<ContactModel>());

        }

        static (IEnumerable<T> contactsInLocalDatabaseButNotStoredRemotely,
            IEnumerable<T> contactsInRemoteDatabaseButNotStoredLocally,
            IEnumerable<T> contactsInBothDatabases) GetMatchingModels<T>(IEnumerable<T> modelListFromLocalDatabase,
                                                                      IEnumerable<T> modelListFromRemoteDatabase) where T : IBaseModel
        {
            var modelIdFromRemoteDatabaseList = modelListFromRemoteDatabase?.Select(x => x.Id) ?? Enumerable.Empty<string>();
            var modelIdFromLocalDatabaseList = modelListFromLocalDatabase?.Select(x => x.Id) ?? Enumerable.Empty<string>();

            var modelIdsInRemoteDatabaseButNotStoredLocally = modelIdFromRemoteDatabaseList?.Except(modelIdFromLocalDatabaseList) ?? Enumerable.Empty<string>();
            var modelIdsInLocalDatabaseButNotStoredRemotely = modelIdFromLocalDatabaseList?.Except(modelIdFromRemoteDatabaseList) ?? Enumerable.Empty<string>();
            var modelIdsInBothDatabases = modelIdFromRemoteDatabaseList?.Where(x => modelIdFromLocalDatabaseList?.Contains(x) ?? false) ?? Enumerable.Empty<string>();

            var modelsInRemoteDatabaseButNotStoredLocally = modelListFromRemoteDatabase?.Where(x => modelIdsInRemoteDatabaseButNotStoredLocally?.Contains(x?.Id) ?? false) ?? Enumerable.Empty<T>();
            var modelsInLocalDatabaseButNotStoredRemotely = modelListFromLocalDatabase?.Where(x => modelIdsInLocalDatabaseButNotStoredRemotely?.Contains(x?.Id) ?? false) ?? Enumerable.Empty<T>();

            var modelsInBothDatabases = modelListFromLocalDatabase?.Where(x => modelIdsInBothDatabases?.Contains(x?.Id) ?? false)
                                             ?? Enumerable.Empty<T>();

            return (modelsInLocalDatabaseButNotStoredRemotely ?? Enumerable.Empty<T>(),
                    modelsInRemoteDatabaseButNotStoredLocally ?? Enumerable.Empty<T>(),
                    modelsInBothDatabases ?? Enumerable.Empty<T>());

        }

        static (IEnumerable<T> contactsToPatchToLocalDatabase,
                IEnumerable<T> contactsToPatchToRemoteDatabase) GetModelsThatNeedUpdating<T>(IEnumerable<T> modelListFromLocalDatabase,
                                                                              IEnumerable<T> modelListFromRemoteDatabase,
                                                                              IEnumerable<T> modelsFoundInBothDatabases) where T : IBaseModel
        {
            var modelsToPatchToRemoteDatabase = Enumerable.Empty<T>().ToList();
            var modelsToPatchToLocalDatabase = Enumerable.Empty<T>().ToList();
            foreach (var contact in modelsFoundInBothDatabases)
            {
                var modelFromLocalDatabase = modelListFromLocalDatabase.Where(x => x.Id.Equals(contact.Id)).FirstOrDefault();
                var modelFromRemoteDatabase = modelListFromRemoteDatabase.Where(x => x.Id.Equals(contact.Id)).FirstOrDefault();

                if (modelFromLocalDatabase?.UpdatedAt.CompareTo(modelFromRemoteDatabase?.UpdatedAt ?? default) > 0)
                    modelsToPatchToRemoteDatabase.Add(modelFromLocalDatabase);
                else if (modelFromLocalDatabase?.UpdatedAt.CompareTo(modelFromRemoteDatabase?.UpdatedAt ?? default) < 0)
                    modelsToPatchToLocalDatabase.Add(modelFromRemoteDatabase);
            }

            return (modelsToPatchToLocalDatabase ?? Enumerable.Empty<T>(),
                    modelsToPatchToRemoteDatabase ?? Enumerable.Empty<T>());
        }

        static Task SaveContacts(IEnumerable<ContactModel> contactsToPatchToRemoteDatabase,
                                    IEnumerable<ContactModel> contactsToPatchToLocalDatabase,
                                    IEnumerable<ContactModel> contactsToAddToLocalDatabase,
                                    IEnumerable<ContactModel> contactsToPostToRemoteDatabase)
        {
            var saveContactTaskList = new List<Task>();
            foreach (var contact in contactsToPostToRemoteDatabase)
                saveContactTaskList.Add(ApiService.PostContactModel(contact));

            foreach (var contact in contactsToAddToLocalDatabase)
                saveContactTaskList.Add(ContactDatabase.SaveContact(contact));

            foreach (var contact in contactsToPatchToRemoteDatabase)
                saveContactTaskList.Add(ApiService.PatchContactModel(contact));

            foreach (var contact in contactsToPatchToLocalDatabase)
                saveContactTaskList.Add(ContactDatabase.PatchContactModel(contact));

            return Task.WhenAll(saveContactTaskList);
        }
    }
}
