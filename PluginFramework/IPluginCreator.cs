using System;
using System.Collections.Generic;

namespace PluginFramework
{
  /// <summary>
  /// Interface for creating instances of plugins described by <see cref="PluginDescriptor"/> inside a target <see cref="AppDomain"/>.
  /// </summary>
  interface IPluginCreator
  {
    T Create<T>(PluginDescriptor pluginDescriptor, AppDomain domain = null, Dictionary<string, object> settings = null) where T : class;
  }
}
