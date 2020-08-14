using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Analytics;
using XamList.Mobile.Shared;

namespace XamList
{
    public class AppCenterService : IAnalyticsService
    {
        public void Start() => Start(AppCenterConstants.ApiKey);

        public void Track(string trackIdentifier, IDictionary<string, string>? table = null) =>
            Analytics.TrackEvent(trackIdentifier, table);

        public void Track(string trackIdentifier, string key, string value)
        {
            IDictionary<string, string>? table = new Dictionary<string, string> { { key, value } };

            if (string.IsNullOrWhiteSpace(key) && string.IsNullOrWhiteSpace(value))
                table = null;

            Track(trackIdentifier, table);
        }

        public void Report(Exception exception,
                                  IDictionary<string, string>? properties = null,
                                  [CallerMemberName] string callerMemberName = "",
                                  [CallerLineNumber] int lineNumber = 0,
                                  [CallerFilePath] string filePath = "")
        {
            PrintException(exception, callerMemberName, lineNumber, filePath);

            Crashes.TrackError(exception, properties);
        }

        [Conditional("DEBUG")]
        void PrintException(Exception exception, string callerMemberName, int lineNumber, string filePath)
        {
            var fileName = System.IO.Path.GetFileName(filePath);

            Debug.WriteLine(exception.GetType());
            Debug.WriteLine($"Error: {exception.Message}");
            Debug.WriteLine($"Line Number: {lineNumber}");
            Debug.WriteLine($"Caller Name: {callerMemberName}");
            Debug.WriteLine($"File Name: {fileName}");
        }

        void Start(string appSecret) => AppCenter.Start(appSecret, typeof(Analytics), typeof(Crashes));
    }
}
