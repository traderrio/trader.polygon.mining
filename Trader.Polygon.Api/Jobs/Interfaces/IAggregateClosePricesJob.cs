using System.Threading.Tasks;

namespace Trader.Polygon.Api.Jobs.Interfaces
{
    public interface IAggregateClosePricesJob
    {
        Task Aggregate();
    }
}