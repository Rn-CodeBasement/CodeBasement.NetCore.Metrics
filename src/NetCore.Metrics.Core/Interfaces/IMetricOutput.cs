using System.Collections.Generic;
using CodeBasement.NetCore.Metrics.Core.Models;

namespace CodeBasement.NetCore.Metrics.Core.Interfaces
{
  public interface IMetricOutput
  {
    bool Enabled { get; }

    void SendMetrics(IEnumerable<LineProtocolPoint> metrics);
  }
}
