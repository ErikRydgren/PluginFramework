using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace PluginFramework
{
  /// <summary>
  /// Interface for exposing changes in a plugin container
  /// </summary>
  public interface IPluginSource
  {
    event EventHandler<PluginEventArgs> PluginAdded;
    event EventHandler<PluginEventArgs> PluginRemoved;
  }

  public class PluginEventArgs : EventArgs
  {
    public PluginEventArgs(PluginDescriptor plugin)
    {
      this.Plugin = plugin;
    }

    public PluginDescriptor Plugin { get; private set; }
  }
}
