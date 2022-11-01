using AutoFixture;
using AutoFixture.AutoMoq;

namespace Dzaba.AdCheck.TestUtils
{
    public static class TestFixture
    {
        public static IFixture CreateFixture()
        {
            return new Fixture().Customize(new AutoMoqCustomization());
        }
    }
}
