﻿using System;
using System.Linq;

using Xamarin.UITest;
using Xamarin.UITest.Queries;
using Xamarin.UITest.iOS;
using Xamarin.UITest.Android;

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
    }
}

