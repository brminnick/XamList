using Xamarin.Forms;

namespace XamList
{
    public abstract class BaseContentPage<TViewModel> : ContentPage where TViewModel : BaseViewModel, new()
    {
        #region Constructors
        protected BaseContentPage()
        {
            BindingContext = ViewModel;
            BackgroundColor = ColorConstants.PageBackgroundColor;
        }
        #endregion

        #region Properties
        protected TViewModel ViewModel { get; } = new TViewModel();
        #endregion

        #region Methods
        protected abstract void SubscribeEventHandlers();
        protected abstract void UnsubscribeEventHandlers();

        protected override void OnAppearing()
        {
            base.OnAppearing();

            SubscribeEventHandlers();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            UnsubscribeEventHandlers();
        }
        #endregion
    }
}
