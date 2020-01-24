using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using CodeBasement.NetCore.Common.Factories.Interfaces;
using CodeBasement.NetCore.Common.Logging;
using CodeBasement.NetCore.Common.Wrappers.Interfaces;
using CodeBasement.NetCore.Metrics.Core.Interfaces;
using CodeBasement.NetCore.Metrics.Core.Models;

namespace CodeBasement.NetCore.Metrics.Core
{
  public class MetricService : IMetricService
  {
    private readonly ILoggerAdapter<MetricService> _logger;
    private readonly ConcurrentQueue<LineProtocolPoint> _metrics;
    private readonly ITimer _flushTimer;
    private readonly List<IMetricOutput> _outputs;

    public MetricService(
      ILoggerAdapter<MetricService> logger,
      ITimerFactory timerFactory,
      IEnumerable<IMetricOutput> outputs)
    {
      // TODO: [TESTS] (MetricService.MetricService) Add tests
      // TODO: [LOGGING] (MetricService.MetricService) Add logging

      _logger = logger;

      _metrics = new ConcurrentQueue<LineProtocolPoint>();
      _outputs = outputs.Where(o => o.Enabled).ToList();

      // TODO: [CONFIG] (MetricService.MetricService) Make this configurable
      _flushTimer = timerFactory.CreateTimer(1000);
      _flushTimer.Elapsed += FlushMetrics;
      _flushTimer.Start();
    }


    // Public methods
    public void EnqueueMetric(LineProtocolPoint point)
    {
      // TODO: [TESTS] (MetricService.EnqueueMetric) Add tests
      // TODO: [LOGGING] (MetricService.EnqueueMetric) Add logging
      // TODO: [CHECK] (MetricService.EnqueueMetric) Don't enqueue metrics if there is no enabled output

      _metrics.Enqueue(point);
    }


    // Internal methods
    private void FlushMetrics(object sender, ElapsedEventArgs e)
    {
      // TODO: [TESTS] (MetricService.FlushMetrics) Add tests
      if (_metrics.IsEmpty) return;

      // Stop the flush timer and log
      _flushTimer.Stop();

      _logger.LogTrace(
        "Flushing {mCount} queued metric(s) on {oCount} output(s)",
        _metrics.Count,
        _outputs.Count);

      // Dequeue metrics to send
      var metrics = new List<LineProtocolPoint>();
      while (!_metrics.IsEmpty)
      {
        if (_metrics.TryDequeue(out var entry))
        {
          metrics.Add(entry);
        }
      }

      // Send metrics to all enabled outputs
      foreach (var output in _outputs)
      {
        output.SendMetrics(metrics);
      }

      // Restart the flush timer
      _flushTimer.Start();
    }
  }
}
