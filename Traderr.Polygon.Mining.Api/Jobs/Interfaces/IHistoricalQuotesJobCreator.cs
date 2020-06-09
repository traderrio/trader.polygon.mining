using System.Threading.Tasks;

namespace Traderr.Polygon.Mining.Api.Jobs.Interfaces
{
    public interface IHistoricalQuotesJobCreator
    {
        Task Collect();
    }
}