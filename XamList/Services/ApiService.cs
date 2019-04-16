using Xamarin.Forms;

using XamList.Mobile.Shared;

namespace XamList
{
    class ApiService : BaseApiService<ApiService>
    {
        public ApiService() : base(Device.RuntimePlatform)
        {
        }
    }
}
