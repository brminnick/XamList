using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

using Newtonsoft.Json;

using Xamarin.Forms;

namespace XamList
{
    abstract class BaseHttpClientService
    {
        #region Constant Fields
        static readonly JsonSerializer _serializer = new JsonSerializer();
        static readonly HttpClient _client = CreateHttpClient(TimeSpan.FromSeconds(60));
        #endregion

        #region Fields
        static int _networkIndicatorCount = 0;
        #endregion

        #region Methods
        protected static Task<T> GetDataObjectFromAPI<T>(string apiUrl) =>
            GetDataObjectFromAPI<T, object>(apiUrl);

        protected static async Task<TDataObject> GetDataObjectFromAPI<TDataObject, TPayloadData>(string apiUrl, TPayloadData data = default(TPayloadData))
        {
            var stringPayload = string.Empty;

            if (data != null)
                stringPayload = await Task.Run(() => JsonConvert.SerializeObject(data)).ConfigureAwait(false);

            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            try
            {
                UpdateActivityIndicatorStatus(true);

                using (var stream = await _client.GetStreamAsync(apiUrl).ConfigureAwait(false))
                using (var reader = new StreamReader(stream))
                using (var json = new JsonTextReader(reader))
                {
                    if (json == null)
                        return default(TDataObject);

                    return await Task.Run(() => _serializer.Deserialize<TDataObject>(json)).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                MobileCenterHelpers.Log(e);
                return default(TDataObject);
            }
            finally
            {
                UpdateActivityIndicatorStatus(false);
            }
        }

        protected static async Task<HttpResponseMessage> PostObjectToAPI<T>(string apiUrl, T data)
        {
            var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(data)).ConfigureAwait(false);

            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            try
            {
                UpdateActivityIndicatorStatus(true);

                return await _client.PostAsync(apiUrl, httpContent).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                MobileCenterHelpers.Log(e);
                return null;
            }
            finally
            {
                UpdateActivityIndicatorStatus(false);
            }
        }

        protected static async Task<HttpResponseMessage> PatchObjectToAPI<T>(string apiUrl, T data)
        {
            var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(data)).ConfigureAwait(false);

            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            var httpRequest = new HttpRequestMessage
            {
                Method = new HttpMethod("PATCH"),
                RequestUri = new Uri(apiUrl),
                Content = httpContent
            };

            try
            {
                UpdateActivityIndicatorStatus(true);

                return await _client.SendAsync(httpRequest).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                MobileCenterHelpers.Log(e);
                return null;
            }
            finally
            {
                UpdateActivityIndicatorStatus(false);
            }
        }

        protected static Task<HttpResponseMessage> DeleteObjectFromAPI(string apiUrl)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Delete, new Uri(apiUrl));

            try
            {
                UpdateActivityIndicatorStatus(true);

                return _client.SendAsync(httpRequest);
            }
            catch (Exception e)
            {
                MobileCenterHelpers.Log(e);
                return null;
            }
            finally
            {
                UpdateActivityIndicatorStatus(false);
            }
        }

        static void UpdateActivityIndicatorStatus(bool isActivityIndicatorDisplayed)
        {
            var baseNavigationPage = Application.Current.MainPage as NavigationPage;
            var currentPage = baseNavigationPage.CurrentPage as ContentPage;
            var currentViewModel = currentPage.BindingContext as BaseViewModel;

            if (isActivityIndicatorDisplayed)
            {
                currentViewModel.IsInternetConnectionActive = true;
                _networkIndicatorCount++;
            }
            else if (--_networkIndicatorCount <= 0)
            {
                currentViewModel.IsInternetConnectionActive = false;
                _networkIndicatorCount = 0;
            }
        }

        static HttpClient CreateHttpClient(TimeSpan timeout)
        {
            HttpClient client;

            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                case Device.Android:
                    client = new HttpClient();
                    break;
                default:
                    client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });
                    break;

            }
            client.Timeout = timeout;
            client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            return client;
        }
        #endregion
    }
}
