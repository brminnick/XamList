using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace XamList.UnitTests
{
    public class MockConnectivity : IConnectivity
    {
        public NetworkAccess NetworkAccess { get; } = NetworkAccess.Internet;

        public IEnumerable<ConnectionProfile> ConnectionProfiles { get; } = new[]
        {
            ConnectionProfile.WiFi,
            ConnectionProfile.Unknown
        };

        public event EventHandler<ConnectivityChangedEventArgs>? ConnectivityChanged;
    }
}
