using Xamarin.UITest;

using XamList.Constants;

using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace XamList.UITests
{
    public class ContactDetailsPage : BasePage
    {
        #region Constant Fields
        readonly Query _firstNameEntry, _lastNameEntry, _phoneNumberEntry,
            _saveButton, _cancelButton;
        #endregion

        #region Constructors
        public ContactDetailsPage(IApp app, Platform platform) : base(app, platform, PageTitles.ContactDetailsPage)
        {
            _firstNameEntry = x => x.Marked(AutomationIdConstants.FirstNameEntry);
            _lastNameEntry = x => x.Marked(AutomationIdConstants.LastNameEntry);
            _phoneNumberEntry = x => x.Marked(AutomationIdConstants.PhoneNumberEntry);
            _saveButton = x => x.Marked(AutomationIdConstants.SaveContactButton);
            _cancelButton = x => x.Marked(AutomationIdConstants.CancelContactButton);
        }
        #endregion

        #region Methods
        public void EnterFirstNameText(string text, bool shouldUseReturnKey) =>
            EnterText(_firstNameEntry, text, shouldUseReturnKey);

        public void EnterLastNameText(string text, bool shouldUseReturnKey) =>
            EnterText(_lastNameEntry, text, shouldUseReturnKey);

        public void EnterPhoneNumberText(string text, bool shouldUseReturnKey) =>
            EnterText(_phoneNumberEntry, text, shouldUseReturnKey);

        public void PopulateAllTextFields(string firstName, string lastName, string phoneNumber, bool shouldUseReturnKey)
        {
            EnterFirstNameText(firstName, shouldUseReturnKey);
            EnterLastNameText(lastName, shouldUseReturnKey);
            EnterPhoneNumberText(phoneNumber, shouldUseReturnKey);
        }

        public void TapSaveButton()
        {
			switch (OniOS)
			{
				case true:
					App.Tap(_saveButton);
					break;

				default:
					App.Tap("Save");
					break;
			}

            App.Screenshot("Save Button Tapped");
        }

        public void TapCancelButton()
        {
            switch (OniOS)
            {
                case true:
                    App.Tap(_cancelButton);
                    break;
                
                default:
                    App.Tap("Cancel");
                    break;
            }

            App.Screenshot("Cancel Button Tapped");
        }

        void EnterText(Query query, string text, bool shouldUseReturnKey)
        {
            switch (shouldUseReturnKey)
            {
                case true:
                    EnterTextThenTapEnter(query, text);
                    break;
                default:
                    EnterTextThenDismissKeyboard(query, text);
                    break;
            }
        }

        void EnterTextThenDismissKeyboard(Query query, string text)
        {
            App.EnterText(query, text);
            App.DismissKeyboard();
            App.Screenshot($"Entered Text: {text}");
        }

        void EnterTextThenTapEnter(Query query, string text)
        {
            App.Tap(query);

            App.ClearText();
            App.EnterText(text);

            App.Screenshot($"Entered Text: {text}");

            App.PressEnter();
        }
        #endregion
    }
}
