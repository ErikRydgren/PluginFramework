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
    FileInfo assemblyFile;
    AssemblyReflectionManager reflectionManager;

    public AssemblyAddedEventArgs(FileInfo assemblyFile, AssemblyReflectionManager reflectionManager)
    {
      this.assemblyFile = assemblyFile;
      this.reflectionManager = reflectionManager;
    }

    public string AssemblyFullName
    {
      get
      {
        return this.Reflect(assembly => assembly.FullName);
      }
    }

    public T Reflect<T>(Func<Assembly, T> reflector)
    {
      return reflectionManager.Reflect(this.assemblyFile.FullName, reflector);
    }
  }
  public class AssemblyRemovedEventArgs : EventArgs
  {
    public AssemblyRemovedEventArgs(string assemblyFullName)
    {
      this.AssemblyFullName = assemblyFullName;
    }

    public string AssemblyFullName { get; private set; }
  }
}
