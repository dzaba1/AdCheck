using AutoFixture;
using NUnit.Framework;
using Dzaba.AdCheck.TestUtils;
using Dzaba.AdCheck.ActiveDirectory.Contracts;
using Dzaba.AdCheck.DataAccess.Contracts;
using Moq;
using FluentAssertions;

namespace Dzaba.AdCheck.Diff.UnitTests
{
    [TestFixture]
    public class DifferTests
    {
        private IFixture fixture;
        private Mock<IUsersDal> usersDal;
        private Mock<IPollingDal> pollingsDal;

        [SetUp]
        public void Setup()
        {
            fixture = TestFixture.CreateFixture();

            usersDal = fixture.FreezeMock<IUsersDal>();
            pollingsDal = fixture.FreezeMock<IPollingDal>();
        }

        private Differ CreateSut()
        {
            return fixture.Create<Differ>();
        }

        private void SetupUsersForPolling(int pollingId, params AdUser[] users)
        {
            usersDal.Setup(x => x.GetFromPolling(pollingId))
                .Returns(users);
        }

        private IPolling SetupPollingById(int id, DateTime datetime)
        {
            var polling = new Mock<IPolling>();
            polling.Setup(x => x.Id).Returns(id);
            polling.Setup(x => x.TimeStamp).Returns(datetime);

            pollingsDal.Setup(x => x.GetPolling(id))
                .Returns(polling.Object);

            return polling.Object;
        }

        private AdUser CreateUser(string domain = "domain", string sidOverride = null, bool empty = false)
        {
            var user = empty ? new AdUser() : fixture.Create<AdUser>();
            user.Domain = domain;

            if (empty)
            {
                user.Dn = "Dn";
            }

            if (!string.IsNullOrWhiteSpace(sidOverride))
            {
                user.Sid = sidOverride;
            }

            return user;
        }

        [Test]
        public void Diff_WhenUsersProvidedWithTheSameSid_ThenChanges()
        {
            var sid = "sid";
            var user1 = CreateUser(sidOverride: sid);
            var user2 = CreateUser(sidOverride: sid);

            var polling1Id = 1;
            var polling2Id = 2;

            SetupPollingById(polling1Id, DateTime.Now.AddDays(-1));
            SetupPollingById(polling2Id, DateTime.Now);
            SetupUsersForPolling(polling1Id, user1);
            SetupUsersForPolling(polling2Id, user2);

            var sut = CreateSut();
            var result = sut.Diff(polling1Id, polling2Id, new Contracts.DiffFiltering()).Single();
            result.Added.Should().BeEmpty();
            result.Deleted.Should().BeEmpty();
            result.Changes.Should().HaveCount(1);
            result.Changes[sid].Changes.Should().HaveCount(20);
        }

        [Test]
        public void Diff_WhenUsersProvidedWithDifferentSid_ThenDeleteAndAdd()
        {
            var user1 = CreateUser();
            var user2 = CreateUser();

            var polling1Id = 1;
            var polling2Id = 2;

            SetupPollingById(polling1Id, DateTime.Now.AddDays(-1));
            SetupPollingById(polling2Id, DateTime.Now);
            SetupUsersForPolling(polling1Id, user1);
            SetupUsersForPolling(polling2Id, user2);

            var sut = CreateSut();
            var result = sut.Diff(polling1Id, polling2Id, new Contracts.DiffFiltering()).Single();
            result.Changes.Should().BeEmpty();
            result.Added.Single().User.Should().Be(user2);
            result.Deleted.Single().User.Should().Be(user1);
        }

        [Test]
        public void Diff_WhenUsersAddedButAddedIgnored_ThenNothing()
        {
            var user1 = CreateUser();
            var user2 = CreateUser();

            var polling1Id = 1;
            var polling2Id = 2;

            SetupPollingById(polling1Id, DateTime.Now.AddDays(-1));
            SetupPollingById(polling2Id, DateTime.Now);
            SetupUsersForPolling(polling1Id, user1);
            SetupUsersForPolling(polling2Id, user2);

            var options = new Contracts.DiffFiltering
            {
                Selection = new Contracts.DiffSelectionOptions
                {
                    IgnoreAdded = true
                }
            };

            var sut = CreateSut();
            var result = sut.Diff(polling1Id, polling2Id, options).Single();
            result.Changes.Should().BeEmpty();
            result.Added.Should().BeEmpty();
            result.Deleted.Single().User.Should().Be(user1);
        }

        [Test]
        public void Diff_WhenUsersDeletedButDeletedIgnored_ThenNothing()
        {
            var user1 = CreateUser();
            var user2 = CreateUser();

            var polling1Id = 1;
            var polling2Id = 2;

            SetupPollingById(polling1Id, DateTime.Now.AddDays(-1));
            SetupPollingById(polling2Id, DateTime.Now);
            SetupUsersForPolling(polling1Id, user1);
            SetupUsersForPolling(polling2Id, user2);

            var options = new Contracts.DiffFiltering
            {
                Selection = new Contracts.DiffSelectionOptions
                {
                    IgnoreDeleted = true
                }
            };

            var sut = CreateSut();
            var result = sut.Diff(polling1Id, polling2Id, options).Single();
            result.Changes.Should().BeEmpty();
            result.Added.Single().User.Should().Be(user2);
            result.Deleted.Should().BeEmpty();
        }

        [Test]
        public void Diff_WhenUsersChangesButChangedIgnored_ThenNothing()
        {
            var sid = "sid";
            var user1 = CreateUser(sidOverride: sid);
            var user2 = CreateUser(sidOverride: sid);

            var polling1Id = 1;
            var polling2Id = 2;

            SetupPollingById(polling1Id, DateTime.Now.AddDays(-1));
            SetupPollingById(polling2Id, DateTime.Now);
            SetupUsersForPolling(polling1Id, user1);
            SetupUsersForPolling(polling2Id, user2);

            var options = new Contracts.DiffFiltering
            {
                Selection = new Contracts.DiffSelectionOptions
                {
                    IgnoreChanges = true
                }
            };

            var sut = CreateSut();
            var result = sut.Diff(polling1Id, polling2Id, options).Single();
            result.Added.Should().BeEmpty();
            result.Deleted.Should().BeEmpty();
            result.Changes.Should().BeEmpty();
        }

        [Test]
        public void Diff_WhenUsersChangesButPropertiesToCompareSpecified_ThenNothing()
        {
            var sid = "sid";
            var name = "Some name";
            var user1 = CreateUser(sidOverride: sid);
            user1.FirstName = name;

            var user2 = CreateUser(sidOverride: sid);
            user2.FirstName = name;

            var polling1Id = 1;
            var polling2Id = 2;

            SetupPollingById(polling1Id, DateTime.Now.AddDays(-1));
            SetupPollingById(polling2Id, DateTime.Now);
            SetupUsersForPolling(polling1Id, user1);
            SetupUsersForPolling(polling2Id, user2);

            var options = new Contracts.DiffFiltering
            {
                 DiffOptions = new Contracts.DiffOptions
                 {
                     PropertiesToCompare = new[] {nameof (IAdUser.FirstName)}
                 }
            };

            var sut = CreateSut();
            var result = sut.Diff(polling1Id, polling2Id, options).Single();
            result.Added.Should().BeEmpty();
            result.Deleted.Should().BeEmpty();
            result.Changes.Should().BeEmpty();
        }

        [Test]
        public void Diff_WhenUsersAddedAndFilteringSpecifiedAsEquals_ThenFiltering()
        {
            var name = "Some name";
            var user1 = CreateUser();
            user1.FirstName = name;

            var user2 = CreateUser();

            var polling1Id = 1;
            var polling2Id = 2;

            SetupPollingById(polling1Id, DateTime.Now.AddDays(-1));
            SetupPollingById(polling2Id, DateTime.Now);
            SetupUsersForPolling(polling1Id);
            SetupUsersForPolling(polling2Id, user1, user2);

            var options = new Contracts.DiffFiltering
            {
                Search = new Contracts.DiffSearchOptions
                {
                    UserConditions = new[]
                    {
                        new Contracts.SearchCondition
                        {
                            Name = nameof(IAdUser.FirstName),
                            Operator = Contracts.SearchOperator.Equal,
                            Value = name
                        }
                    }
                }
            };

            var sut = CreateSut();
            var result = sut.Diff(polling1Id, polling2Id, options).Single();
            result.Changes.Should().BeEmpty();
            result.Added.Single().User.Should().Be(user1);
            result.Deleted.Should().BeEmpty();
        }

        [Test]
        public void Diff_WhenUsersAddedAndFilteringSpecifiedAsAny_ThenFiltering()
        {
            var user1 = CreateUser();
            var user2 = CreateUser();
            user2.FirstName = null;

            var polling1Id = 1;
            var polling2Id = 2;

            SetupPollingById(polling1Id, DateTime.Now.AddDays(-1));
            SetupPollingById(polling2Id, DateTime.Now);
            SetupUsersForPolling(polling1Id);
            SetupUsersForPolling(polling2Id, user1, user2);

            var options = new Contracts.DiffFiltering
            {
                Search = new Contracts.DiffSearchOptions
                {
                    UserConditions = new[]
                    {
                        new Contracts.SearchCondition
                        {
                            Name = nameof(IAdUser.FirstName),
                            Operator = Contracts.SearchOperator.Any
                        }
                    }
                }
            };

            var sut = CreateSut();
            var result = sut.Diff(polling1Id, polling2Id, options).Single();
            result.Changes.Should().BeEmpty();
            result.Added.Single().User.Should().Be(user1);
            result.Deleted.Should().BeEmpty();
        }

        [Test]
        public void Diff_WhenChangeWithFiltering_ThenNothing()
        {
            var sid = "sid";
            var user1 = CreateUser(sidOverride: sid, empty: true);
            user1.FirstName = "Left";
            user1.Surname = "Left";

            var user2 = CreateUser(sidOverride: sid, empty: true);
            user2.FirstName = "Right";
            user2.Surname = "Right";

            var polling1Id = 1;
            var polling2Id = 2;

            SetupPollingById(polling1Id, DateTime.Now.AddDays(-1));
            SetupPollingById(polling2Id, DateTime.Now);
            SetupUsersForPolling(polling1Id, user1);
            SetupUsersForPolling(polling2Id, user2);

            var options = new Contracts.DiffFiltering
            {
                Search = new Contracts.DiffSearchOptions
                {
                    ChangeConditions = new[]
                    {
                        new Contracts.SearchChangeCondition
                        {
                            Name = nameof(IAdUser.FirstName),
                            Operator = Contracts.SearchOperator.Equal,
                            Value = "Invalid"
                        }
                    }
                }
            };

            var sut = CreateSut();
            var result = sut.Diff(polling1Id, polling2Id, options).Single();
            result.Added.Should().BeEmpty();
            result.Deleted.Should().BeEmpty();
            result.Changes.Should().BeEmpty();
        }
    }
}
