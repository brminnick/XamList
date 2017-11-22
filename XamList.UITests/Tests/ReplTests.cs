using NUnit.Framework;

using Xamarin.UITest;

namespace XamList.UITests
{
    public class ReplTests : BaseTest
    {
        #region Fields
        IApp _app;
        #endregion

        #region Constructors
        public ReplTests(Platform platform) : base(platform) { }
        #endregion

        #region Methods
        protected override void BeforeEachTest() => _app = AppInitializer.StartApp(Platform);

        [Ignore]
        [Test]
        public void Repl() => _app.Repl();
        #endregion
    }
}
