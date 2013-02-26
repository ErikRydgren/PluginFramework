using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace PluginFramework
{
  /// <summary>
  /// Interface for reporting changes in a collection of assemblies
  /// </summary>
  public interface IAssemblySource
  {
    event EventHandler<AssemblyAddedEventArgs> AssemblyAdded;
    event EventHandler<AssemblyRemovedEventArgs> AssemblyRemoved;
  }

  public class AssemblyAddedEventArgs : EventArgs
  {
    AssemblyReflectionManager reflectionManager;

    public AssemblyAddedEventArgs(string id, AssemblyReflectionManager reflectionManager)
    {
      this.AssemblyId = id;
      this.reflectionManager = reflectionManager;
    }

    public string AssemblyId { get; private set; }

    public T Reflect<T>(Func<Assembly, T> reflector)
    {
      return reflectionManager.Reflect(AssemblyId, reflector);
    }
  }
  public class AssemblyRemovedEventArgs : EventArgs
  {
    public AssemblyRemovedEventArgs(string assemblyId)
    {
      this.AssemblyId = assemblyId;
    }

    public string AssemblyId { get; private set; }
  }
}
