﻿using System;
using System.Linq;

using Xamarin.UITest;
using Xamarin.UITest.Queries;

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
        string GetTitle(int timeoutInSeconds = 60)
        {
            App.WaitForElement(_pageTitleText, "Could Not Retrieve Page Title", TimeSpan.FromSeconds(timeoutInSeconds));

            AppResult[] titleQuery;
            switch (OniOS)
            {
                case true:
                    titleQuery = App.Query(x => x.Class("UILabel").Marked(_pageTitleText));
                    break;

                default:
                    titleQuery = App.Query(x => x.Class("AppCompatTextView").Marked(_pageTitleText));
                    break;
            }

            return titleQuery?.FirstOrDefault()?.Text;
        }
        #endregion
    }
}

