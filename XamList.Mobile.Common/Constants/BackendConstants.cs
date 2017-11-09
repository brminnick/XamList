namespace XamList.Mobile.Common
{
#warning: To utilize your own Azure API and Azure Function, replace the values of these strings
    public static class BackendConstants
    {
        public const string AzureAPIUrl = "https://xamlistapi.azurewebsites.net/api/";
        public const string AzureFunctionUrl = "https://xamlistfunctions.azurewebsites.net/api/";
        public const string AzureFunctionKey_RemoveItemFromDatabase= "qZGcFbpqxBTpdz4K0f45m81qS9eHzSOMBWjGH5o1SfH8cycnYbaf3Q==";
        public const string AzureFunctionKey_RestoreDeletedContacts = "Mnl87ggoCqlHjMrieftOpq5gSL4BJHfmMT76tq87RbAmC6gaehcL2g==";
	}
}
