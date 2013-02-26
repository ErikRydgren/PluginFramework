using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace PluginFramework
{
  /// <summary>
  /// Implementation of <see cref="IPluginRepository"/>.
  /// </summary>
  public class PluginRepository : IPluginRepository
  {
    ISet<PluginDescriptor> plugins;

    public PluginRepository()
    {
      this.plugins = new HashSet<PluginDescriptor>();      
    }

    public void AddPluginSource(IPluginSource pluginSource)
    {
      pluginSource.PluginAdded += this.OnPluginFound;
      pluginSource.PluginRemoved += this.OnPluginLost;
    }

    public void RemovePluginSource(IPluginSource pluginSource)
    {
      pluginSource.PluginAdded -= this.OnPluginFound;
      pluginSource.PluginRemoved -= this.OnPluginLost;
    }

    void OnPluginFound(object sender, PluginEventArgs e)
    {
      this.plugins.Add(e.Plugin);
    }

    void OnPluginLost(object sender, PluginEventArgs e)
    {
      this.plugins.Remove(e.Plugin);
    }

    public IEnumerable<PluginDescriptor> Plugins(PluginFilter filter = null)
    {
      return filter != null ? filter.Filter(this.plugins) : this.plugins;
    }
  }
}
