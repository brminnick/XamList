using NUnit.Framework;

using Xamarin.UITest;

namespace XamList.UITests
{
    public class ReplTests : BaseTest
    {
        public ReplTests(Platform platform) : base(platform)
        {
        }

        [Ignore]
        [Test]
        public void Repl()
        {
            App.Repl();
        }
    }
}
