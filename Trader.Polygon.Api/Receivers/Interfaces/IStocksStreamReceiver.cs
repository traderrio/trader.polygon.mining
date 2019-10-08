using System.Threading.Tasks;

namespace Trader.Polygon.Api.Receivers.Interfaces
{
    public interface IStocksStreamReceiver
    {
        Task ConnectAsync();
    }
}