using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginFramework
{
  /// <summary>
  /// Repository for fetching assemblies by fullname
  /// </summary>
  public interface IAssemblyRepository
  {
    byte[] Get(string assemblyFullName);
  }
}
