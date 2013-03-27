using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginFramework.Logging
{
  public static class ConfiguratorExtension
  {
    public static Configurator UseLog4NetLogger(this Configurator configurator)
    {
      Logger.Singleton.LoggerFactory = new Log4NetIntegration.Log4NetLoggerFactory();
      return configurator;
    }
  }
}
