using AutoFixture;
using Dzaba.AdCheck.Utils;
using Moq;

namespace Dzaba.AdCheck.TestUtils
{
    public static class Extensions
    {
        public static Mock<T> FreezeMock<T>(this IFixture fixture) where T : class
        {
            Require.NotNull(fixture, nameof(fixture));

            return fixture.Freeze<Mock<T>>();
        }
    }
}
