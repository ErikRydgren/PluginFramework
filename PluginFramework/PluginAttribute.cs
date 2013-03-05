// 
// Copyright (c) 2013, Erik Rydgren, et al. All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
//  - Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//  - Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the 
//    documentation and/or other materials provided with the distribution.
//  - Neither the name of PluginFramework nor the names of its contributors may be used to endorse or promote products derived from this 
//    software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL ERIK RYDGREN OR OTHER CONTRIBUTORS 
// BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
//
namespace PluginFramework
{
  using System;

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

    public PluginVersion(string versionstring)
      : this()
    {
      try
      {
        string[] parts = versionstring.Split(new char[] { '.' });
        if (parts.Length != 2)
          throw new FormatException();
        this.Major = int.Parse(parts[0]);
        this.Minor = int.Parse(parts[1]);
      }
      catch (FormatException)
      {
        throw new FormatException("The versionstring must be on format intMajor.intMinor");
      }
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

    public static explicit operator PluginVersion(string versionstring)
    {
      return new PluginVersion(versionstring);
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
