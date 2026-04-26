using System.Threading.Channels;

namespace TradingEngine.MatchingEngine;

public sealed class MatchingEngineOptions
{
    /// <summary>Number of independent workers (shards). Each shard processes a disjoint set of symbols.</summary>
    public int ShardCount { get; set; } = Math.Max(1, Environment.ProcessorCount / 2);

    /// <summary>Bounded capacity per shard channel.</summary>
    public int ChannelCapacity { get; set; } = 10_000;

    /// <summary>Behavior when a shard queue is full.</summary>
    public BoundedChannelFullMode FullMode { get; set; } = BoundedChannelFullMode.Wait;
}
