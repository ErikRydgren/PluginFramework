using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginFramework
{
  /// <summary>
  /// Makes a class detectable as a plugin and provides metadata used for finding the plugin
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited=false)]
  public class PluginAttribute : Attribute
  {
    public string Name { get; set; }
  }

  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class PluginVersionAttribute : Attribute
  {
    int major;
    int minor;

    public PluginVersionAttribute(int major, int minor)
    {
      this.major = major;
      this.minor = minor;
    }
  }

  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class PluginInfoAttribute : Attribute
  {
    string id;
    string value;

    public PluginInfoAttribute(string id, string value)
    {
      this.id = id;
      this.value = value;
    }
  }


  /// <summary>
  /// Marks a plugin property as a plugin setting
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
  public class PluginSettingAttribute : Attribute
  {
    public PluginSettingAttribute()
    {
      this.Required = false;
    }

    public bool Required { get; set; }
    public string Name { get; set; }
  }

  /// <summary>
  /// Used to describe a plugin version
  /// </summary>
  [Serializable]
  public struct PluginVersion : IComparable<PluginVersion>, IComparable, IEquatable<PluginVersion>
  {
    public int Major { get; set; }
    public int Minor { get; set; }

    public PluginVersion(int major, int minor)
      : this()
    {
      this.Major = major;
      this.Minor = minor;
    }

    public int CompareTo(PluginVersion other)
    {
      if (this.Major != other.Major)
        return this.Major - other.Major;
      return this.Minor - other.Minor;
    }

    int IComparable.CompareTo(object obj)
    {
      return this.CompareTo((PluginVersion)obj);
    }

    public bool Equals(PluginVersion other)
    {
      return this.CompareTo(other) == 0;
    }

    public override string ToString()
    {
      return string.Format("{0}.{1}", this.Major, this.Minor);
    }

    public static implicit operator PluginVersion(string versionstring)
    {
      try
      {
        string[] parts = versionstring.Split(new char[] { '.' });
        if (parts.Length != 2)
          throw new FormatException();
        PluginVersion version = new PluginVersion(int.Parse(parts[0]), int.Parse(parts[1]));
        return version;
      }
      catch (FormatException)
      {
        throw new FormatException("The versionstring must be on format intMajor.intMinor");
      }
    }

    public static bool operator < (PluginVersion left, PluginVersion right)
    {
      return left.CompareTo(right) < 0;
    }

    public static bool operator <= (PluginVersion left, PluginVersion right)
    {
      return left.CompareTo(right) <= 0;
    }

    public static bool operator >(PluginVersion left, PluginVersion right)
    {
      return left.CompareTo(right) > 0;
    }

    public static bool operator >=(PluginVersion left, PluginVersion right)
    {
      return left.CompareTo(right) >= 0;
    }

  }
}
