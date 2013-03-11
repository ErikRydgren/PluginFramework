using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginFramework.Tests
{
  internal class MockPluginSource : IPluginSource
  {
    EventHandler<PluginEventArgs> pluginAdded;
    EventHandler<PluginEventArgs> pluginRemoved;

    public event EventHandler<PluginEventArgs> PluginAdded
    {
      add
      {
        this.pluginAdded += value;
        NumPluginAddedListeners++;
      }

      remove
      {
        this.pluginAdded -= value;
        NumPluginAddedListeners--;
      }
    }

    public event EventHandler<PluginEventArgs> PluginRemoved
    {
      add
      {
        this.pluginRemoved += value;
        NumPluginRemovedListeners++;
      }

      remove
      {
        this.pluginRemoved -= value;
        NumPluginRemovedListeners--;
      }
    }

    public int NumPluginAddedListeners { get; set; }
    public int NumPluginRemovedListeners { get; set; }

    public void RaisePluginAdded(PluginDescriptor descriptor)
    {
      if (this.pluginAdded == null)
        return;

      this.pluginAdded(this, new PluginEventArgs(descriptor));
    }

    public void RaisePluginRemoved(PluginDescriptor descriptor)
    {
      if (this.pluginRemoved == null)
        return;

      this.pluginRemoved(this, new PluginEventArgs(descriptor));
    }
  }
}
