using System;
using System.Collections.Generic;
using System.Linq;
using Dzaba.AdCheck.DataAccess.Contracts;
using Dzaba.AdCheck.DataAccess.Contracts.Model;
using Dzaba.AdCheck.DataAccess.Repository;
using Dzaba.AdCheck.Utils;
using Microsoft.Extensions.Logging;

namespace Dzaba.AdCheck.DataAccess.Dal
{
    internal sealed class PollingDal : IPollingDal
    {
        private readonly ILogger<PollingDal> logger;
        private readonly IRepositoryFactory repoFactory;

        public PollingDal(ILogger<PollingDal> logger,
            IRepositoryFactory repoFactory)
        {
            Require.NotNull(logger, nameof(logger));
            Require.NotNull(repoFactory, nameof(repoFactory));

            this.logger = logger;
            this.repoFactory = repoFactory;
        }

        public int NewPolling()
        {
            logger.LogInformation("Saving a new polling.");

            using (var context = repoFactory.Create<Polling>())
            {
                var entity = new Polling
                {
                    TimeStamp = DateTime.UtcNow
                };
                context.Insert(entity);
                context.SaveChanges();

                return entity.Id;
            }
        }

        public IPolling GetPolling(DateTime history)
        {
            using (var context = repoFactory.Create<Polling>())
            {
                var polling = context.Get()
                    .OrderByDescending(p => p.TimeStamp)
                    .FirstOrDefault(p => p.TimeStamp <= history);
                if (polling == null)
                {
                    throw new InvalidOperationException($"Can't find data from the date {history}");
                }

                return polling;
            }
        }

        public IPolling GetPolling(int id)
        {
            using (var context = repoFactory.Create<Polling>())
            {
                var polling = context.Get()
                    .OrderByDescending(p => p.TimeStamp)
                    .FirstOrDefault(p => p.Id == id);
                if (polling == null)
                {
                    throw new InvalidOperationException($"Can't find polling with ID {id}");
                }

                return polling;
            }
        }

        public IPolling GetLatest()
        {
            using (var context = repoFactory.Create<Polling>())
            {
                var polling = context.Get()
                    .OrderByDescending(p => p.TimeStamp)
                    .FirstOrDefault();
                if (polling == null)
                {
                    throw new InvalidOperationException("There aren't any history.");
                }

                return polling;
            }
        }

        public IEnumerable<IPolling> GetAll()
        {
            using (var context = repoFactory.Create<Polling>())
            {
                return context.Get().ToArray();
            }
        }
    }
}
