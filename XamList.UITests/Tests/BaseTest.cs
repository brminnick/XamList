﻿ using NUnit.Framework;

using Xamarin.UITest;

namespace XamList.UITests
{
    [TestFixture(Platform.iOS)]
    [TestFixture(Platform.Android)]
    public abstract class BaseTest
    {
        #region Fields
        IApp _app;
        Platform _platform;
        ContactsListPage _contactsListPage;
        ContactDetailsPage _contactsDetailsPage;
        #endregion

        #region Constructors
        protected BaseTest(Platform platform) => _platform = platform;
        #endregion

        #region Properties
		protected Platform Platform => _platform;
        protected IApp App => _app;
        protected ContactsListPage ContactsListPage => _contactsListPage ?? (_contactsListPage = new ContactsListPage(App, Platform));
        protected ContactDetailsPage ContactDetailsPage => _contactsDetailsPage ?? (_contactsDetailsPage = new ContactDetailsPage(App, Platform));
        #endregion

        #region Methods
        [SetUp]
        protected virtual void BeforeEachTest() =>  _app = AppInitializer.StartApp(Platform);
        #endregion
    }
}

