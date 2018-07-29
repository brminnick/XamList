using System;

namespace XamList
{
    public static class AppCenterConstants
    {
        #region Constant Fields
        public const string ContactDetailPageAppeared = "Contact Detail Page Appeared";
        public const string PullToRefreshTriggered = "Pull To Refresh Triggered";

        public const string ContactsListPageAppeared = "Contacts List Page Appeared";
        public const string AddContactButtonTapped = "Add Contact Button Tapped";
        public const string RestoreDeletedContactsTapped = "Restore Deleted Contacts Tapped";

        const string AppCenterAPIKey_iOS = "44a3c601-0552-4fd8-91cd-092eaff8cab8";
        const string AppCenterAPIKey_Droid = "62c4425c-d8b4-482a-a7f8-f8ec88c6500f";
        #endregion

        #region Properties
        public static string ApiKey => GetAppCenterApiKey();
        #endregion

        #region Methods
        static string GetAppCenterApiKey()
        {
            switch (Xamarin.Forms.Device.RuntimePlatform)
            {
                case Xamarin.Forms.Device.iOS:
                    return AppCenterAPIKey_iOS;
                case Xamarin.Forms.Device.Android:
                    return AppCenterAPIKey_Droid;
                default:
                    throw new PlatformNotSupportedException();
            }
        }
        #endregion
    }
}
