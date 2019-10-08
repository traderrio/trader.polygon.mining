using System.Threading.Tasks;
using Trader.Polygon.Core.Streaming.Messages;

namespace Trader.Polygon.Api.Pipelines.Interfaces
{
    public interface IStreamMessagePipelineProcessor<in T> where T : StreamingMessage
    {
        Task Process(T record);
    }
}