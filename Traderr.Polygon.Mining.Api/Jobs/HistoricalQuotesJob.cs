using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Hangfire;
using Hangfire.Storage;
using MessagePack;
using Microsoft.Extensions.Options;
using Traderr.Polygon.Mining.Api.Core;
using Traderr.Polygon.Mining.Api.Core.Infrastructure.Jobs.Interfaces;
using Traderr.Polygon.Mining.Api.Jobs.Interfaces;
using Traderr.Polygon.Mining.Api.Settings;

namespace Traderr.Polygon.Mining.Api.Jobs
{
    public class HistoricalQuotesJobCreator : IHistoricalQuotesJobCreator
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IJobRunner<ICollectStockHistoryQuotesJob> _jobRunner;
        private const string JobPrefix = "Historical-Quotes-";

        public HistoricalQuotesJobCreator(IOptions<AppSettings> appSettings,
            IJobRunner<ICollectStockHistoryQuotesJob> jobRunner
        )
        {
            _appSettings = appSettings;
            _jobRunner = jobRunner;
        }

        public async Task Collect()
        {
            var recJobs = JobStorage.Current.GetConnection().GetRecurringJobs();
            var currentHistoricalJobs = recJobs.Where(r => r.Id.StartsWith(JobPrefix)).ToList();
            

            var url = $"{_appSettings.Value.Microservices.TraderrApi}/api/integration/historical-tickers";
            var response = await url.GetJsonAsync<BinaryIntegrationResponse>();

            var historicalTickers = MessagePackSerializer.Deserialize<IList<HistoricalTicker>>(response.Result,
                MessagePack.Resolvers.ContractlessStandardResolver.Instance);

            foreach (var currentHistoricalJob in currentHistoricalJobs)
            {
                if (historicalTickers.Count(t => $"{JobPrefix}{t.Ticker}" == currentHistoricalJob.Id) == 0)
                {
                    _jobRunner.RemoveRecurringTask(currentHistoricalJob.Id);
                }
            }

            foreach (var historicalTicker in historicalTickers)
            {
                _jobRunner.AddRecurringTask($"{JobPrefix}{historicalTicker.Ticker}", 
                    j=>j.Collect(historicalTicker), () => Cron.Daily(0));
            }
        }
    }

    public class HistoricalTicker
    {
        public string Ticker { get; set; }
        public DateTime StartDate { get; set; }
    }
}