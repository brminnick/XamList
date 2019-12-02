using System;
using System.Linq;
using Xamarin.UITest;
using Xamarin.UITest.Android;
using Xamarin.UITest.iOS;

namespace XamList.UITests
{
    public abstract class BasePage
    {
        readonly string _pageTitleText;

        protected BasePage(IApp app, string pageTitle)
        {
            App = app;
            _pageTitleText = pageTitle;
        }

        public string Title => GetTitle();
        protected IApp App { get; }

        public void WaitForPageToLoad() => App.WaitForElement(_pageTitleText);

        string GetTitle(int timeoutInSeconds = 60)
        {
            App.WaitForElement(_pageTitleText, "Could Not Retrieve Page Title", TimeSpan.FromSeconds(timeoutInSeconds));

            var titleQuery = App switch
            {
                iOSApp iosApp => iosApp.Query(x => x.Class("UILabel").Marked(_pageTitleText)),
                AndroidApp androidApp => androidApp.Query(x => x.Class("AppCompatTextView").Marked(_pageTitleText)),
                _ => throw new NotSupportedException(),
            };

            return titleQuery.First().Text;
        }
    }
}

