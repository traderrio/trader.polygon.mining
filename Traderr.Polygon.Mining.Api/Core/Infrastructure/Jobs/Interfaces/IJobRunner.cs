using System;
using System.Linq.Expressions;

namespace Traderr.Polygon.Mining.Api.Core.Infrastructure.Jobs.Interfaces
{
	public interface IJobRunner<T>
	{
		void AddTask(Expression<Action<T>> task);

		void AddRecurringTask(string jobId, Expression<Action<T>> task, Func<string> cron,
			TimeZoneInfo timeZoneInfo = null);

		void RemoveRecurringTask(string jobId);

		void TriggerJob(string jobId);
	}
}