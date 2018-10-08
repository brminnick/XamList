namespace XamList.Shared
{
#warning: To utilize your own Azure API and Azure Function, replace the values of these strings
    public static class BackendConstants
    {
        public const string AzureAPIUrl = "https://xamlistapicore.azurewebsites.net/api/";
        public const string AzureFunctionUrl = "https://xamlistfunctions2.azurewebsites.net/";
        public const string AzureFunctionKey_RemoveItemFromDatabase = "12rEQESsyX2qOw0aKgK2WQ8mgjDbdp6XKiwPNCM04WyXCY9Ep9Wr5A==";
        public const string AzureFunctionKey_RestoreDeletedContacts = "O/6QQypqCzsJgdh5s6KxNkE393tksrZLePK/eszd4JwHPFkHmuI7nA==";
    }
}
