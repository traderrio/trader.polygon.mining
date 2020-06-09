using Hangfire;
using Traderr.Polygon.Mining.Api.Core.Infrastructure.Jobs.Interfaces;
using Traderr.Polygon.Mining.Api.Jobs.Interfaces;

namespace Traderr.Polygon.Mining.Api.Jobs
{
    public class JobFactory : IJobFactory
    {
        private readonly IJobRunner<IHistoricalQuotesJobCreator> _historicalQuoteJobRunner;

        public JobFactory(IJobRunner<IHistoricalQuotesJobCreator> historicalQuoteJobRunner)
        {
            _historicalQuoteJobRunner = historicalQuoteJobRunner;
        }

        public void CreateCollectHistoryQuoteData()
        {
            var jobId = "HistoricalQuotesJobCreator";
            _historicalQuoteJobRunner.AddRecurringTask(jobId, t=>t.Collect(), () => Cron.Daily(23));
            _historicalQuoteJobRunner.TriggerJob(jobId);
        }
    }
}