using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Crashes;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Distribute;
using Microsoft.Azure.Mobile.Push;

namespace XamList
{
    public static class MobileCenterHelpers
    {
        public static void Start()
        {
            switch (Xamarin.Forms.Device.RuntimePlatform)
            {
                case Xamarin.Forms.Device.iOS:
                    Task.Run(async () => await Start(MobileCenterConstants.MobileCenterAPIKey_iOS));
                    break;
                case Xamarin.Forms.Device.Android:
                    Task.Run(async () => await Start(MobileCenterConstants.MobileCenterAPIKey_Droid));
                    break;
                default:
                    throw new Exception("Runtime Platform Not Supported");
            }
        }

        public static void TrackEvent(string trackIdentifier, IDictionary<string, string> table = null) =>
            Analytics.TrackEvent(trackIdentifier, table);

        public static void TrackEvent(string trackIdentifier, string key, string value)
        {
            IDictionary<string, string> table = new Dictionary<string, string> { { key, value } };

            if (string.IsNullOrWhiteSpace(key) && string.IsNullOrWhiteSpace(value))
                table = null;

            TrackEvent(trackIdentifier, table);
        }

        public static void Log(Exception exception, MobileCenterLogType type = MobileCenterLogType.Warn)
        {
            var exceptionType = exception.GetType().ToString();
            var message = exception.Message;

            System.Diagnostics.Debug.WriteLine(exceptionType);
            System.Diagnostics.Debug.WriteLine($"Error: {message}");

            switch (type)
            {
                case MobileCenterLogType.Info:
                    MobileCenterLog.Info(exceptionType, message, exception);
                    break;
                case MobileCenterLogType.Warn:
                    MobileCenterLog.Warn(exceptionType, message, exception);
                    break;
                case MobileCenterLogType.Error:
                    MobileCenterLog.Error(exceptionType, message, exception);
                    break;
                case MobileCenterLogType.Assert:
                    MobileCenterLog.Assert(exceptionType, message, exception);
                    break;
                case MobileCenterLogType.Verbose:
                    MobileCenterLog.Verbose(exceptionType, message, exception);
                    break;
                case MobileCenterLogType.Debug:
                    MobileCenterLog.Debug(exceptionType, message, exception);
                    break;
                default:
                    throw new Exception("MobileCenterLogType Does Not Exist");
            }
        }


		static async Task Start(string appSecret)
		{
			MobileCenter.Start(appSecret, typeof(Analytics), typeof(Crashes), typeof(Distribute), typeof(Push));
#if DEBUG
			await Distribute.SetEnabledAsync(false);
#else
            await Distribute.SetEnabledAsync(true);
#endif
			await Analytics.SetEnabledAsync(true);
		}
    }

    public enum MobileCenterLogType
    {
        Assert, Debug, Error, Info, Verbose, Warn
    }

}
