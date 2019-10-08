using System;
using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Trader.Polygon.Api.Jobs.Interfaces;
using Trader.Polygon.Api.Receivers.Interfaces;
using Trader.Polygon.Api.Services.Interfaces;

namespace Trader.Polygon.Api
{
    partial class Startup
    {
        private void Bootstrap(IServiceProvider serviceProvider)
        {
            //var priceMemoryHolder = serviceProvider.GetRequiredService<ILastStockPriceMemoryHolder>();
            //priceMemoryHolder.LoadRecentAsync().Wait();

            //var stocksStreamReceiver = serviceProvider.GetRequiredService<IStocksStreamReceiver>();
            //stocksStreamReceiver.ConnectAsync();

            //var jobFactory = serviceProvider.GetRequiredService<IHangfireJobFactory>();
            //jobFactory.CreateAggCloseJob();
        }

        private void JobManagerOnJobException(JobExceptionInfo exceptionInfo)
        {
            _logger.LogError(exceptionInfo.Exception, exceptionInfo.Name);
        }
    }
}