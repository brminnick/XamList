using Xamarin.Forms;

namespace XamList
{
    public class DependencyServiceHelpers : IDependencyService
    {
        public T Get<T>() where T : class => DependencyService.Get<T>();
    }
}
