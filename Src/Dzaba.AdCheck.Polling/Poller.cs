using Dzaba.AdCheck.Polling.Contracts;
using Dzaba.AdCheck.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Dzaba.AdCheck.ActiveDirectory.Contracts;
using Dzaba.AdCheck.DataAccess.Contracts;
using Microsoft.Extensions.Logging;

namespace Dzaba.AdCheck.Polling
{
    internal sealed class Poller : IPoller
    {
        private readonly IAdDal adDal;
        private readonly ILogger<Poller> logger;
        private readonly IUsersDal usersDal;
        private readonly IPollingDal pollingDal;
        private readonly PollOptions options;

        public Poller(ILogger<Poller> logger,
            IUsersDal usersDal,
            IAdDal adDal,
            IPollingDal pollingDal,
            PollOptions options)
        {
            Require.NotNull(logger, nameof(logger));
            Require.NotNull(adDal, nameof(adDal));
            Require.NotNull(usersDal, nameof(usersDal));
            Require.NotNull(pollingDal, nameof(pollingDal));
            Require.NotNull(options, nameof(options));

            this.logger = logger;
            this.usersDal = usersDal;
            this.adDal = adDal;
            this.pollingDal = pollingDal;
            this.options = options;
        }

        public void DownloadAll(IEnumerable<string> domains)
        {
            Require.NotNull(domains, nameof(domains));

            var perfWatch = Stopwatch.StartNew();

            var pollingId = pollingDal.NewPolling();
            if (options.ParallelDomainsProcessing)
            {
                logger.LogDebug("Parallel domains processing is enabled.");
                Parallel.ForEach(domains, domain =>
                {
                    ProcessDomain(domain, pollingId);
                });
            }
            else
            {
                foreach (var domain in domains)
                {
                    ProcessDomain(domain, pollingId);
                }
            }

            perfWatch.Stop();
            logger.LogInformation("Finished downloading from all domains. Took: {Elapsed}", perfWatch.Elapsed);
        }

        private void ProcessDomain(string domain, int pollingId)
        {
            try
            {
                var perfWatch = Stopwatch.StartNew();

                var users = adDal.GetAllUsers(domain, options.ParallelAdUsersProcessing);
                usersDal.SaveUsers(users, pollingId);

                perfWatch.Stop();
                logger.LogInformation("Finished processing domain '{Domain}'. Took: {Elapsed}", domain,
                    perfWatch.Elapsed);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing domain '{Domain}'", domain);
            }
        }
    }
}
