using System;
using System.Collections.Generic;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;

namespace Traderr.Polygon.Mining.Api.Core.Infrastructure
{
	public class SkipWhenPreviousJobIsRunningAttribute : JobFilterAttribute, IClientFilter, IApplyStateFilter
	{
		public void OnCreating(CreatingContext context)
		{
			if (!(context.Connection is JobStorageConnection connection)) return;

			if (!context.Parameters.ContainsKey("RecurringJobId")) return;

			var recurringJobId = context.Parameters["RecurringJobId"] as string;

			if (String.IsNullOrWhiteSpace(recurringJobId)) return;

			var running = connection.GetValueFromHash($"recurring-job:{recurringJobId}", "Running");
			if ("yes".Equals(running, StringComparison.OrdinalIgnoreCase))
			{
				context.Canceled = true;
			}
		}

		public void OnCreated(CreatedContext filterContext)
		{
		}

		public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
		{
			if (context.NewState is EnqueuedState)
			{
				var recurringJobId = SerializationHelper.Deserialize<string>(context.Connection.GetJobParameter(context.BackgroundJob.Id, "RecurringJobId"));
				if (String.IsNullOrWhiteSpace(recurringJobId)) return;

				transaction.SetRangeInHash(
					$"recurring-job:{recurringJobId}",
					new[] { new KeyValuePair<string, string>("Running", "yes") });
			}
			else if (context.NewState.IsFinal)
			{
				var recurringJobId = SerializationHelper.Deserialize<string>(context.Connection.GetJobParameter(context.BackgroundJob.Id, "RecurringJobId"));
				if (String.IsNullOrWhiteSpace(recurringJobId)) return;

				transaction.SetRangeInHash(
					$"recurring-job:{recurringJobId}",
					new[] { new KeyValuePair<string, string>("Running", "no") });
			}
		}

		public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
		{
		}
	}
}
