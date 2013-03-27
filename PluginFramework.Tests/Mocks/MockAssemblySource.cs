using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginFramework.Tests.Mocks
{
  public class MockAssemblySource : IAssemblySource
  {
    event EventHandler<AssemblyAddedEventArgs> assemblyAdded;
    event EventHandler<AssemblyRemovedEventArgs> assemblyRemoved;

    public int AssemblyAddedSubscribers { get; set; }
    public int AssemblyRemovedSubscribers { get; set; }

    public void RaiseAssemblyAdded(AssemblyAddedEventArgs args)
    {
      if (this.assemblyAdded != null)
        this.assemblyAdded(this, args);
    }

    public void RaiseAssemblyRemoved(AssemblyRemovedEventArgs args)
    {
      if (this.assemblyRemoved != null)
        this.assemblyRemoved(this, args);
    }

    public event EventHandler<AssemblyAddedEventArgs> AssemblyAdded
    {
      add
      {
        AssemblyAddedSubscribers++;
        assemblyAdded += value;
      }

      remove
      {
        AssemblyAddedSubscribers--;
        assemblyAdded -= value;
      }
    }

    public event EventHandler<AssemblyRemovedEventArgs> AssemblyRemoved
    {
      add
      {
        AssemblyRemovedSubscribers++;
        assemblyRemoved += value;
      }

      remove
      {
        AssemblyRemovedSubscribers--;
        assemblyRemoved -= value;
      }
    }
  }
}
