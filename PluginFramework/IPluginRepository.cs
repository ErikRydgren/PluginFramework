using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginFramework
{
  /// <summary>
  /// Provides methods for finding plugins with specified properties.
  /// </summary>
  public interface IPluginRepository
  {
    IEnumerable<PluginDescriptor> Plugins(PluginFilter filter = null);
    IEnumerable<PluginDescriptor> Plugins<T>(PluginFilter filter = null);
  }
}
