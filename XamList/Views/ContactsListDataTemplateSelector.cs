using Xamarin.Forms;
using Xamarin.Forms.Markup;
using XamList.Shared;
using static XamList.MarkupExtensions;
using static Xamarin.Forms.Markup.GridRowsColumns;

namespace XamList
{
    class ContactsListDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => new ContactsListDataTemplate((ContactModel)item);

        class ContactsListDataTemplate : DataTemplate
        {
            public ContactsListDataTemplate(ContactModel contactModel) : base(() => CreateDataTemplate(contactModel))
            {

            }

            static View CreateDataTemplate(in ContactModel contactModel) => new Grid
            {
                RowSpacing = 2,

                RowDefinitions = Rows.Define(
                    (Row.Text, Star),
                    (Row.Detail, Star),
                    (Row.Divider, AbsoluteGridLength(11))),

                Children =
                {
                    new TextLabel(contactModel.FullName, ColorConstants.TextColor, 16).Row(Row.Text),

                    new TextLabel(contactModel.PhoneNumber, ColorConstants.DetailColor, 13).Row(Row.Detail),

                    new BoxView { Color = Color.DarkGray }.Margin(5, 5).Row(Row.Divider)
                }
            };

            enum Row { Text, Detail, Divider }

            class TextLabel : Label
            {
                public TextLabel(in string text, in Color textColor, in double fontSize)
                {
                    Text = text;
                    FontSize = fontSize;
                    TextColor = textColor;

                    Margin = 0;
                    Padding = new Thickness(10, 0);
                }
            }
        }
    }
}
