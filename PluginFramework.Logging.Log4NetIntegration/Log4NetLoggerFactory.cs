using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginFramework.Logging.Log4NetIntegration
{
  public class Log4NetLoggerFactory : ILoggerFactory
  {
    public ILog GetLog(string name)
    {
      return new Log4NetLogger(log4net.LogManager.GetLogger(name));
    }
  }
}
