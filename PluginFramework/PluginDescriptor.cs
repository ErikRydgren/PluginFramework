using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginFramework
{
  /// <summary>
  /// Describes a plugin. Used by a <see cref="IPluginCreator"/> to create instances of the plugin.
  /// </summary>
  [Serializable]
  public class PluginDescriptor : IEquatable<PluginDescriptor>
  {
    public QualifiedName QualifiedName { get; set; }
    public QualifiedName[] Derives { get; set; }
    public QualifiedName[] Interfaces { get; set; }
    public PluginSettingDescriptor[] settings { get; set; }

    public string Name { get; set; }
    public PluginVersion Version { get; set; }

    public bool Equals(PluginDescriptor other)
    {
      return this.QualifiedName == other.QualifiedName;
    }
  }

  /// <summary>
  /// Describes a plugin setting. 
  /// </summary>
  [Serializable]
  public class PluginSettingDescriptor
  {
    public string Name { get; set; }
    public QualifiedName Type { get; set; }
    public bool Required { get; set; }

    public override string ToString()
    {
      return string.Format("{0} [{1}] {2}", this.Name, this.Required ? "required" : "optional", this.Type);
    }
  }
}
