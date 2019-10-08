using Hangfire;
using Trader.Polygon.Api.Jobs.Interfaces;
using Trader.Services.Interfaces;

namespace Trader.Polygon.Api.Jobs
{
    public class HangfireJobFactory : IHangfireJobFactory
    {
        private readonly IJobRunner<IAggregateClosePricesJob> _aggJobRunner;

        public HangfireJobFactory(IJobRunner<IAggregateClosePricesJob> aggJobRunner)
        {
            _aggJobRunner = aggJobRunner;
        }

        public void CreateAggCloseJob()
        {
            _aggJobRunner.AddRecurringTask("AggClose", t=>t.Aggregate(), () => Cron.Daily(8));
        }
    }
}