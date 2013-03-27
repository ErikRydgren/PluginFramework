using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginFramework.Logging
{
  public static class ConfiguratorExtension
  {
    public static Configurator UseNLogLogger(this Configurator configurator)
    {
      Logger.Singleton.LoggerFactory = new NLogIntegration.NLogLoggerFactory();
      return configurator;
    }
  }
}
