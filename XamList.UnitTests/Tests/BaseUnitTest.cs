using NUnit.Framework;

using System.Threading.Tasks;

namespace XamList.UnitTests
{
    public abstract class BaseUnitTest
    {
        [SetUp]
        protected virtual Task BeforeEachTest() => Task.CompletedTask;
    }
}
