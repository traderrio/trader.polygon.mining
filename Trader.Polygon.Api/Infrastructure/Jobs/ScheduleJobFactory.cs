using System;
using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;

namespace Trader.Polygon.Api.Infrastructure.Jobs
{
    public class ScheduleJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ScheduleJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob GetJobInstance<T>() where T : IJob
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}