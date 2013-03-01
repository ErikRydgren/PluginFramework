using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginFramework
{
  /// <summary>
  /// Utility class for managing fully assembly qualified type names
  /// </summary>
  [Serializable]
  public struct QualifiedName
  {
    public QualifiedName(string qualifiedName)
      : this()
    {
      var parts = Split(qualifiedName);
      this.TypeFullName = parts[0];
      this.AssemblyFullName = parts[1];
    }

    public string TypeFullName
    {
      get; private set; 
    }

    public string AssemblyFullName
    {
      get; private set;
    }

    public override string ToString()
    {
      return this;
    }

    public static string[] Split(string qualifiedName)
    {
      string[] delimiters = new string[] { ", " };
      string[] nameParts = qualifiedName.Split(delimiters, 2, StringSplitOptions.None);
      return nameParts;
    }

    public static implicit operator QualifiedName(string name)
    {
      return new QualifiedName(name);
    }

    public static implicit operator string(QualifiedName name)
    {
      return name.TypeFullName + ", " + name.AssemblyFullName;
    }
  }
}
