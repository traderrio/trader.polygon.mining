using System;
using System.Linq.Expressions;
using Hangfire;
using Traderr.Polygon.Mining.Api.Core.Infrastructure.Jobs.Interfaces;

namespace Traderr.Polygon.Mining.Api.Core.Infrastructure.Jobs
{
    public class JobRunner<T> : IJobRunner<T>
    {
	    public void AddTask(Expression<Action<T>> task)
	    {
			BackgroundJob.Enqueue<T>(task);
		}

	    public void AddRecurringTask(string jobId, Expression<Action<T>> task, Func<string> cron, 
			TimeZoneInfo timeZoneInfo = null)
	    {
		    RecurringJob.AddOrUpdate(jobId, task, cron, timeZoneInfo);
	    }

	    public void TriggerJob(string jobId)
	    {
		    RecurringJob.Trigger(jobId);
	    }

	    public void RemoveRecurringTask(string jobId)
	    {
			RecurringJob.RemoveIfExists(jobId);
	    }
    }
}
