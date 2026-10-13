using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PublisherEventBus = CqrsSpike.Publisher.Application.Common.Eventing.IMessageBus;
using PublisherUnitOfWork = CqrsSpike.Publisher.Domain.Common.Interfaces.IUnitOfWork;

namespace CqrsSpike.Scenarios.Infrastructure;

public sealed record PipelineEntry(long Sequence, string? TraceId, string What);

/// <summary>
/// Records the observable side effects of the dispatch pipeline (bus buffering, unit-of-work
/// commit, bus flush) tagged with the HTTP request that caused them.
/// </summary>
public sealed class PipelineRecorder
{
    private long _sequence;
    private readonly ConcurrentQueue<PipelineEntry> _entries = new();

    public void Record(IServiceProvider sp, string what)
    {
        var traceId = sp.GetService<IHttpContextAccessor>()?.HttpContext?.TraceIdentifier;
        _entries.Enqueue(new PipelineEntry(Interlocked.Increment(ref _sequence), traceId, what));
    }

    public IReadOnlyList<PipelineEntry> ForTrace(string traceId) =>
        _entries.Where(e => e.TraceId == traceId).OrderBy(e => e.Sequence).ToList();

}

internal sealed class RecordingEventBus : PublisherEventBus
{
    private readonly PublisherEventBus _inner;
    private readonly PipelineRecorder _recorder;
    private readonly IServiceProvider _sp;
    private int _buffered;

    public RecordingEventBus(PublisherEventBus inner, PipelineRecorder recorder, IServiceProvider sp)
    {
        _inner = inner;
        _recorder = recorder;
        _sp = sp;
    }

    public void Publish<TMessage>(TMessage message) where TMessage : class
    {
        _buffered++;
        _recorder.Record(_sp, $"publish:{typeof(TMessage).Name}");
        _inner.Publish(message);
    }

    public void Send<TMessage>(TMessage message) where TMessage : class
    {
        _buffered++;
        _recorder.Record(_sp, $"send:{typeof(TMessage).Name}");
        _inner.Send(message);
    }

    public void Send<TMessage>(TMessage message, Uri address) where TMessage : class
    {
        _buffered++;
        _recorder.Record(_sp, $"send:{typeof(TMessage).Name}");
        _inner.Send(message, address);
    }

    public async Task FlushAllAsync(CancellationToken cancellationToken = default)
    {
        _recorder.Record(_sp, $"flush:{_buffered}");
        _buffered = 0;
        await _inner.FlushAllAsync(cancellationToken);
    }
}

internal sealed class RecordingUnitOfWork : PublisherUnitOfWork
{
    private readonly PublisherUnitOfWork _inner;
    private readonly PipelineRecorder _recorder;
    private readonly IServiceProvider _sp;

    public RecordingUnitOfWork(PublisherUnitOfWork inner, PipelineRecorder recorder, IServiceProvider sp)
    {
        _inner = inner;
        _recorder = recorder;
        _sp = sp;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await _inner.SaveChangesAsync(cancellationToken);
        _recorder.Record(_sp, "commit");
        return result;
    }

    public bool HasDbTransaction() => _inner.HasDbTransaction();
}
