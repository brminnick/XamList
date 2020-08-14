using Xamarin.Essentials.Interfaces;
using XamList.Mobile.Shared;

namespace XamList
{
    public class ConnectivityService
    {
        readonly ApiService _apiService;
        readonly DatabaseSyncService _databaseSyncService;

        public ConnectivityService(ApiService apiService, DatabaseSyncService databaseService, IConnectivity connectivity)
        {
            _apiService = apiService;
            _databaseSyncService = databaseService;
            connectivity.ConnectivityChanged += HandleConnectivityChanged;
        }

        async void HandleConnectivityChanged(object sender, Xamarin.Essentials.ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess is Xamarin.Essentials.NetworkAccess.Internet)
            {
                var apiResponse = await _apiService.GetHttpResponseMessage().ConfigureAwait(false);

                if (apiResponse.IsSuccessStatusCode)
                    await _databaseSyncService.SyncRemoteAndLocalDatabases().ConfigureAwait(false);
            }
        }
    }
}
