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
    public string Name;

    public QualifiedName(string qualifiedName)
    {
      this.Name = qualifiedName;
    }

    public string TypeFullName
    {
      get
      {
        return Split(Name)[0];
      }
    }

    public string AssemblyFullName
    {
      get
      {
        return Split(Name)[1];
      }
    }

    public override string ToString()
    {
      return this.Name;
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
      return name.Name;
    }
  }
}
