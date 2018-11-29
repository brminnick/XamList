using System;
using System.Linq;

using Xamarin.UITest;
using Xamarin.UITest.Queries;
using Xamarin.UITest.iOS;
using Xamarin.UITest.Android;

namespace XamList.UITests
{
    public abstract class BasePage
    {
        #region Constant Fields
        readonly string _pageTitleText;
        #endregion

        #region Constructors
        protected BasePage(IApp app, Platform platform, string pageTitle)
        {
            App = app;

            OnAndroid = platform == Platform.Android;
            OniOS = platform == Platform.iOS;

            _pageTitleText = pageTitle;
        }
        #endregion

        #region Properties
        public string Title => GetTitle();

        protected IApp App { get; }
        protected bool OnAndroid { get; }
        protected bool OniOS { get; }
        #endregion

        #region Methods
        public void WaitForPageToLoad() => App.WaitForElement(_pageTitleText);

        string GetTitle(int timeoutInSeconds = 60)
        {
            App.WaitForElement(_pageTitleText, "Could Not Retrieve Page Title", TimeSpan.FromSeconds(timeoutInSeconds));

            AppResult[] titleQuery;
            switch (App)
            {
                case iOSApp iosApp:
                    titleQuery = iosApp.Query(x => x.Class("UILabel").Marked(_pageTitleText));
                    break;

                case AndroidApp androidApp:
                    titleQuery = androidApp.Query(x => x.Class("AppCompatTextView").Marked(_pageTitleText));
                    break;

                default:
                    throw new NotSupportedException();
            }

            return titleQuery?.FirstOrDefault()?.Text;
        }
        #endregion
    }
}

