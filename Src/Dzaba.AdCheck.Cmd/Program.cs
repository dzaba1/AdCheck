using Dzaba.AdCheck.Diff.Contracts;
using Dzaba.AdCheck.Polling.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Dzaba.AdCheck.Cmd
{
    class Program
    {
        /// <summary>
        /// Downloads data from AD or makes diff.
        /// </summary>
        /// <param name="poll">If specified then it makes polling.</param>
        /// <param name="diff">If specified then it makes the diff JSON report file.</param>
        /// <param name="leftDate">Left date to diff. If null then the latest history from DB will be taken.</param>
        /// <param name="rightDate">Right date to diff. If null then it will take current data from AD based on provided domains.</param>
        /// <param name="outFile">JSON file with diff result.</param>
        static void Main(bool poll,
            bool diff,
            DateTime? leftDate,
            DateTime? rightDate,
            string outFile)
        {
            try
            {
                using (var container = Bootstrapper.BuildContainer())
                {
                    var domains = container.GetRequiredService<IConfiguration>()
                        .GetSection("Domains").Get<string[]>();

                    if (poll)
                    {
                        MakePoll(container, domains);
                    }

                    if (diff)
                    {
                        MakeDiff(container, domains, leftDate, rightDate, outFile);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Environment.ExitCode = -1;
            }
        }

        private static void MakeDiff(IServiceProvider container, IEnumerable<string> domains,
            DateTime? leftDate, DateTime? rightDate, string outFile)
        {
            DiffReport[] diff = null;
            var differ = container.GetRequiredService<IDiffer>();

            if (leftDate != null)
            {
                if (rightDate != null)
                {
                    diff = differ.Diff(leftDate.Value, rightDate.Value, null).ToArray();
                }
                else if (domains != null)
                {
                    diff = differ.DiffWithNow(leftDate.Value, domains, null).ToArray();
                }
            }
            else if (domains != null)
            {
                diff = differ.DiffLatest(domains, null).ToArray();
            }

            if (diff != null)
            {
                var json = JsonConvert.SerializeObject(diff, Formatting.Indented);
                var filepath = outFile;
                if (string.IsNullOrWhiteSpace(filepath))
                {
                    filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "diff.json");
                }

                File.WriteAllText(filepath, json);
            }
        }

        private static void MakePoll(IServiceProvider container, IEnumerable<string> domains)
        {
            container.GetRequiredService<IPoller>().DownloadAll(domains);
        }
    }
}
