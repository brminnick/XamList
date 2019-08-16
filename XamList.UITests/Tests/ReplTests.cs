using NUnit.Framework;

using Xamarin.UITest;
namespace XamList.UITests
{
    [TestFixture(Platform.iOS)]
    [TestFixture(Platform.Android)]
    public class ReplTests
    {
        readonly Platform _platform;
        IApp _app;

        public ReplTests(Platform platform) => _platform = platform;

        [Test, Ignore("REPL only used for manually exploring the app")]
        public void Repl() => _app.Repl();

        [SetUp]
        public void BeforeEachTest() => _app = AppInitializer.StartApp(_platform);
    }
}
