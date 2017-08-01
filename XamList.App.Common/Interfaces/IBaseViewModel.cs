using System.ComponentModel;

namespace XamList.Mobile.Common
{
    public interface IBaseViewModel : INotifyPropertyChanged
    {
        #region Properties
        bool IsInternetConnectionActive { get; set; }
        #endregion
    }
}
