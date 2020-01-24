using CodeBasement.NetCore.Metrics.Core.Models;

namespace CodeBasement.NetCore.Metrics.Core.Interfaces
{
  public interface IMetricService
  {
    void EnqueueMetric(LineProtocolPoint point);
  }
}
