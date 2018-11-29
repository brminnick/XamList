using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

using XamList.Shared;
using XamList.Mobile.Shared;

namespace XamList
{
	abstract class APIService : BaseHttpClientService
	{
		#region Methods
		public static Task<List<ContactModel>> GetAllContactModels() =>
			GetObjectFromAPI<List<ContactModel>>($"{BackendConstants.AzureAPIUrl}GetAllContacts");

		public static Task<ContactModel> GetContactModel(string id) =>
			GetObjectFromAPI<ContactModel>($"{BackendConstants.AzureAPIUrl}GetContact/{id}");

		public static Task<ContactModel> PostContactModel(ContactModel contact) =>
			PostObjectToAPI<ContactModel,ContactModel>($"{BackendConstants.AzureAPIUrl}PostContact", contact);

		public static Task<ContactModel> PatchContactModel(ContactModel contact) =>
            PatchObjectToAPI<ContactModel,ContactModel>($"{BackendConstants.AzureAPIUrl}PatchContact/", contact);

		public static Task<HttpResponseMessage> DeleteContactModel(string id) =>
			DeleteObjectFromAPI($"{BackendConstants.AzureAPIUrl}DeleteContact/{id}");

		public static Task<HttpResponseMessage> RestoreDeletedContacts() =>
			PostObjectToAPI($"{BackendConstants.AzureFunctionUrl}RestoreDeletedContacts/?code={BackendConstants.AzureFunctionKey_RestoreDeletedContacts}", new object());
		#endregion
	}
}
