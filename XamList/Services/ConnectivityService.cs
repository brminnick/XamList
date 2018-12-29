using Xamarin.Essentials;
using XamList.Mobile.Shared;

namespace XamList
{
    public static class ConnectivityService
    {
        static ConnectivityService() =>
            Connectivity.ConnectivityChanged += HandleConnectivityChanged;

        private static async void HandleConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess is NetworkAccess.Internet)
            {
                var apiResponse = await APIService.GetHttpResponseMessage().ConfigureAwait(false);

                if (apiResponse.IsSuccessStatusCode)
                    await DatabaseSyncService.SyncRemoteAndLocalDatabases().ConfigureAwait(false);
            }
        }
    }
}
