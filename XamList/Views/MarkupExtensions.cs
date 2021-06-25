using Xamarin.Forms;

namespace XamList
{
    public static class ElementExtensions
    {
        public static TButton Padding<TButton>(this TButton button, double horizontalSize, double verticalSize) where TButton : Button
        {
            button.Padding = new Thickness(horizontalSize, verticalSize);
            return button;
        }
    }    
}