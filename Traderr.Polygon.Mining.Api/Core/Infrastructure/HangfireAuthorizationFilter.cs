using Hangfire.Dashboard;

namespace Traderr.Polygon.Mining.Api.Core.Infrastructure
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}