using System;
using Trader.Polygon.Api.Pipelines.Interfaces;
using Trader.Polygon.Core.Streaming.Messages;

namespace Trader.Polygon.Api.Infrastructure.Interfaces
{
    public interface IStreamMessagePipelineProcessorResolver
    {
        IStreamMessagePipelineProcessor<T> Resolve<T>(Type processor) where T : StreamingMessage;
    }
}