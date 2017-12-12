using NUnit.Framework;

using Xamarin.UITest;
using XamList.Tests.Shared;

namespace XamList.UITests
{
    [TestFixture(Platform.iOS)]
    [TestFixture(Platform.Android)]
    public abstract class BaseUITest : BaseTest
    {
        #region Constructors
        protected BaseUITest(Platform platform) => Platform = platform;
        #endregion

        #region Properties
        protected Platform Platform { get; private set; }
        protected IApp App { get; private set; }
        protected ContactsListPage ContactsListPage { get; private set; }
        protected ContactDetailsPage ContactDetailsPage { get; private set; }
        #endregion

        #region Methods
        protected override void BeforeEachTest()
        {
            base.BeforeEachTest();

            App = AppInitializer.StartApp(Platform);

            BackdoorMethodHelpers.RemoveTestContactsFromLocalDatabase(App);

            ContactsListPage.WaitForPageToLoad();
            ContactsListPage.WaitForNoPullToRefreshActivityIndicatorAsync().GetAwaiter().GetResult();
        }

        protected override void AfterEachTest()
        {
            base.AfterEachTest();

            BackdoorMethodHelpers.RemoveTestContactsFromLocalDatabase(App);
        }
        #endregion
    }
}

