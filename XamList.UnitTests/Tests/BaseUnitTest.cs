using NUnit.Framework;

using Xamarin.Forms.Mocks;

namespace XamList.UnitTests
{
    public abstract class BaseUnitTest
    {
        #region Properties
        protected DependencyServiceStub DependencyService { get; private set; }
        #endregion

        #region Methods
        [SetUp]
        protected virtual void BeforeEachTest() => MockForms.Init();
        #endregion
    }
}
