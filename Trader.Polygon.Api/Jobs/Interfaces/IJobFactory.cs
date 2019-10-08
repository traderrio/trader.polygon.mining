namespace Trader.Polygon.Api.Jobs.Interfaces
{
    public interface IHangfireJobFactory
    {
        void CreateAggCloseJob();
    }
}