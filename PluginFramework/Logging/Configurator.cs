using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginFramework.Logging
{
  /// <summary>
  /// Exposes configuration helper functions
  /// </summary>
  public class Configurator
  {
    /// <summary>
    /// Setup PluginFramwork to use the trace logger.
    /// </summary>
    public Configurator UseTraceLogger()
    {
      Logger.Singleton.LoggerFactory = new TraceLoggerFactory();
      return this;
    }
  }
}
