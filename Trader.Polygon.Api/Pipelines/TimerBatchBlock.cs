using System;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace Trader.Polygon.Api.Pipelines
{
    public static class TimerBatchBlock
    {
        public static IPropagatorBlock<T, T[]> Create<T>(int batchSize, TimeSpan timeSpan, GroupingDataflowBlockOptions options = null)
        {
            var batchBlock = new BatchBlock<T>(batchSize, options ?? new GroupingDataflowBlockOptions());
            var broadCastBlock = new BroadcastBlock<T[]>(x => x);
            var bufferBlock = new BufferBlock<T[]>();

            var timer = new Timer(x => ((BatchBlock<T>)x).TriggerBatch(), batchBlock, timeSpan, timeSpan);
            
            var resetTimerBlock = new ActionBlock<T[]>(_ => timer.Change(timeSpan, timeSpan)); // reset timer each time buffer outputs
            resetTimerBlock.Completion.ContinueWith(_ => timer.Dispose());

            // link everything up
            var linkOptions = new DataflowLinkOptions()
            {
                PropagateCompletion = true,
            };
            broadCastBlock.LinkTo(resetTimerBlock, linkOptions);
            broadCastBlock.LinkTo(bufferBlock, linkOptions);
            batchBlock.LinkTo(broadCastBlock, linkOptions);

            return DataflowBlock.Encapsulate(batchBlock, bufferBlock);
        }
    }
}