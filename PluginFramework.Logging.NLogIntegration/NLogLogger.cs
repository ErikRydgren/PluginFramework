using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace PluginFramework.Logging.NLogIntegration
{
  /// <summary>
  /// Lets NLog.Logger provide implementation of ILog interface
  /// </summary>
  public class NLogLogger : NLog.Logger, ILog
  {
  }
}
