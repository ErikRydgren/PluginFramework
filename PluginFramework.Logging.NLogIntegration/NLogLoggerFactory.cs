using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginFramework.Logging.NLogIntegration
{
  public class NLogLoggerFactory : ILoggerFactory
  {
    public ILog GetLog(string name)
    {
      return NLog.LogManager.GetLogger(name, typeof(NLogLogger)) as ILog;
    }
  }
}
