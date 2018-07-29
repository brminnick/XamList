using NUnit.Framework;

using System.Threading.Tasks;

namespace XamList.UnitTests
{
    public abstract class BaseUnitTest
    {

        #region Methods
        [SetUp]
        protected virtual Task BeforeEachTest() => Task.CompletedTask;
        #endregion
    }
}
