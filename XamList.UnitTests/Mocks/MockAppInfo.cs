using System;
using AsyncAwaitBestPractices;
using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;

namespace XamList.UnitTests
{
    public class MockAppInfo : IAppInfo
    {
        readonly static WeakEventManager _showSettingsUIInvokedEventManager = new WeakEventManager();

        public static event EventHandler ShowSettingsUIInvoked
        {
            add => _showSettingsUIInvokedEventManager.AddEventHandler(value);
            remove => _showSettingsUIInvokedEventManager.RemoveEventHandler(value);
        }

        public string PackageName { get; } = "com.minnick.gittrends";

        public string Name { get; } = "GitTrends";

        public string VersionString { get; } = "1.1.1";

        public Version Version { get; } = new Version("1.1.1");

        public string BuildString { get; } = "23";

        public AppTheme RequestedTheme { get; } = AppTheme.Light;

        public void ShowSettingsUI() => _showSettingsUIInvokedEventManager.RaiseEvent(this, EventArgs.Empty, nameof(ShowSettingsUIInvoked));
    }
}
