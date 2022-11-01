using FluentAssertions;
using NUnit.Framework;

namespace Dzaba.AdCheck.ActiveDirectory.UnitTests
{
    [TestFixture]
    public class DnTests
    {
        [Test]
        public void Parse_WhenValidDnProvided_ThenDictWithTokens()
        {
            var dn = "CN=dzaba1,OU=Inner Folder,OU=City,OU=Users,OU=SomeCompany,DC=part1,DC=part2";

            var result = Dn.Parse(dn);
            result["cn"].Single().Value.Should().Be("dzaba1");

            var ou = result["ou"];
            ou.Should().HaveCount(4);
            ou[0].Value.Should().Be("Inner Folder");
            ou[1].Value.Should().Be("City");
            ou[2].Value.Should().Be("Users");
            ou[3].Value.Should().Be("SomeCompany");

            var dc = result["dc"];
            dc.Should().HaveCount(2);
            dc[0].Value.Should().Be("part1");
            dc[1].Value.Should().Be("part2");
        }
    }
}
