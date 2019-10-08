using System;
using Microsoft.Extensions.DependencyInjection;
using Trader.Polygon.Api.Infrastructure.Interfaces;
using Trader.Polygon.Api.Pipelines.Interfaces;
using Trader.Polygon.Core.Streaming.Messages;

namespace Trader.Polygon.Api.Infrastructure
{
    public class StreamMessagePipelineProcessorResolver : IStreamMessagePipelineProcessorResolver
    {
        private readonly IServiceProvider _serviceProvider;
        public StreamMessagePipelineProcessorResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IStreamMessagePipelineProcessor<T> Resolve<T>(Type processor) where T : StreamingMessage
        {
            var genericType = processor.MakeGenericType(typeof(T));
            return (IStreamMessagePipelineProcessor<T>) _serviceProvider
                .GetRequiredService(genericType);
        }
    }
}