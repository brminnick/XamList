using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;

using XamList.Mobile.Common;

namespace XamList
{
    public static class ConnectivityService
    {
        public static void SubscribeEventHandlers() =>
            CrossConnectivity.Current.ConnectivityChanged += HandleConnectivityChanged;

        public static void UnsubscribeEventHandlers() =>
            CrossConnectivity.Current.ConnectivityChanged -= HandleConnectivityChanged;

		static async void HandleConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
		{
            var isRemoteDatabaseReachable = CrossConnectivity.Current.IsConnected 
                                            && await CrossConnectivity.Current.IsRemoteReachable(BackendConstants.AzureAPIUrl);

            if (isRemoteDatabaseReachable)
                await DatabaseSyncService.SyncRemoteAndLocalDatabases();
		}
    }
}
