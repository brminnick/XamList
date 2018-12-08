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

		static async Task<(List<ContactModel> contactListFromLocalDatabase,
	        List<ContactModel> contactListFromRemoteDatabase)> GetAllSavedContacts()
		{
			var contactListFromLocalDatabaseTask = ContactDatabase.GetAllContacts();
			var contactListFromRemoteDatabaseTask = APIService.GetAllContactModels();

			await Task.WhenAll(contactListFromLocalDatabaseTask, contactListFromRemoteDatabaseTask).ConfigureAwait(false);

            return (await contactListFromLocalDatabaseTask.ConfigureAwait(false) ?? new List<ContactModel>(),
                    await contactListFromRemoteDatabaseTask.ConfigureAwait(false) ?? new List<ContactModel>());
		}

        static (List<T> contactsInLocalDatabaseButNotStoredRemotely,
            List<T> contactsInRemoteDatabaseButNotStoredLocally,
            List<T> contactsInBothDatabases) GetMatchingModels<T>(List<T> modelListFromLocalDatabase, 
                                                                      List<T> modelListFromRemoteDatabase) where T: IBaseModel
        {
            var modelIdFromRemoteDatabaseList = modelListFromRemoteDatabase?.Select(x => x.Id).ToList() ?? new List<string>();
            var modelIdFromLocalDatabaseList = modelListFromLocalDatabase?.Select(x => x.Id).ToList() ?? new List<string>();

            var modelIdsInRemoteDatabaseButNotStoredLocally = modelIdFromRemoteDatabaseList?.Except(modelIdFromLocalDatabaseList)?.ToList() ?? new List<string>();
            var modelIdsInLocalDatabaseButNotStoredRemotely = modelIdFromLocalDatabaseList?.Except(modelIdFromRemoteDatabaseList)?.ToList() ?? new List<string>();
            var modelIdsInBothDatabases = modelIdFromRemoteDatabaseList?.Where(x => modelIdFromLocalDatabaseList?.Contains(x) ?? false).ToList() ?? new List<string>();

            var modelsInRemoteDatabaseButNotStoredLocally = modelListFromRemoteDatabase?.Where(x => modelIdsInRemoteDatabaseButNotStoredLocally?.Contains(x?.Id) ?? false).ToList() ?? new List<T>();
            var modelsInLocalDatabaseButNotStoredRemotely = modelListFromLocalDatabase?.Where(x => modelIdsInLocalDatabaseButNotStoredRemotely?.Contains(x?.Id) ?? false).ToList() ?? new List<T>();

            var modelsInBothDatabases = modelListFromLocalDatabase?.Where(x => modelIdsInBothDatabases?.Contains(x?.Id) ?? false)
                                            .ToList() ?? new List<T>();

            return (modelsInLocalDatabaseButNotStoredRemotely ?? new List<T>(),
                    modelsInRemoteDatabaseButNotStoredLocally ?? new List<T>(),
                    modelsInBothDatabases ?? new List<T>());

        }

		static (List<T> contactsToPatchToLocalDatabase,
		    List<T> contactsToPatchToRemoteDatabase) GetModelsThatNeedUpdating<T>(List<T> modelListFromLocalDatabase,
																			  List<T> modelListFromRemoteDatabase,
																			  List<T> modelsFoundInBothDatabases) where T : IBaseModel
		{
            var modelsToPatchToRemoteDatabase = new List<T>();
            var modelsToPatchToLocalDatabase = new List<T>();
			foreach (var contact in modelsFoundInBothDatabases)
			{
                var modelFromLocalDatabase = modelListFromLocalDatabase.Where(x => x.Id.Equals(contact.Id)).FirstOrDefault();
                var modelFromRemoteDatabase = modelListFromRemoteDatabase.Where(x => x.Id.Equals(contact.Id)).FirstOrDefault();

				if (modelFromLocalDatabase?.UpdatedAt.CompareTo(modelFromRemoteDatabase?.UpdatedAt ?? default) > 0)
					modelsToPatchToRemoteDatabase.Add(contact);
				else if (modelFromLocalDatabase?.UpdatedAt.CompareTo(modelFromRemoteDatabase?.UpdatedAt ?? default) < 0)
					modelsToPatchToLocalDatabase.Add(contact);
			}

			return (modelsToPatchToLocalDatabase ?? new List<T>(),
					modelsToPatchToRemoteDatabase ?? new List<T>());
		}

        static Task SaveContacts(List<ContactModel> contactsToPatchToRemoteDatabase, 
		                            List<ContactModel> contactsToPatchToLocalDatabase,
		                         List<ContactModel> contactsToAddToLocalDatabase,
                                    List<ContactModel> contactsToPostToRemoteDatabase)
        {
            var saveContactTaskList = new List<Task>();
            foreach (var contact in contactsToPostToRemoteDatabase)
                saveContactTaskList.Add(APIService.PostContactModel(contact));

            foreach (var contact in contactsToAddToLocalDatabase)
                saveContactTaskList.Add(ContactDatabase.SaveContact(contact));

            foreach (var contact in contactsToPatchToRemoteDatabase)
                saveContactTaskList.Add(APIService.PatchContactModel(contact));

            foreach (var contact in contactsToPatchToLocalDatabase)
                saveContactTaskList.Add(ContactDatabase.PatchContactModel(contact));

            return Task.WhenAll(saveContactTaskList);
        }
    }
}
