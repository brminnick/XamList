using Xamarin.Forms.Mocks;

using XamList.Tests.Shared;

namespace XamList.UnitTests
{
    public abstract class BaseUnitTest : BaseTest
    {
        #region Properties
        protected DependencyServiceStub DependencyService { get; private set; }
        #endregion

        #region Methods
        protected override void BeforeEachTest()
        {
            base.BeforeEachTest();

            MockForms.Init();
        }
        #endregion
    }
}
