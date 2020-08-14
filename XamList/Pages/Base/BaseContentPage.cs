using Xamarin.Forms;

namespace XamList
{
    public abstract class BaseContentPage<TViewModel> : ContentPage where TViewModel : BaseViewModel
    {
        protected BaseContentPage(TViewModel viewModel, AppCenterService appCenterService)
        {
            BindingContext = ViewModel = viewModel;

            AppCenterService = appCenterService;

            BackgroundColor = ColorConstants.PageBackgroundColor;
        }

        protected AppCenterService AppCenterService { get; }
        protected TViewModel ViewModel { get; }
    }
}
