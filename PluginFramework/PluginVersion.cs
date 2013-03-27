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
  using System.Globalization;
  /// <summary>
  /// Used to describe a plugin version
  /// </summary>
  [Serializable]
  public struct PluginVersion : IComparable<PluginVersion>, IComparable, IEquatable<PluginVersion>
  {
    /// <summary>
    /// Gets the major version.
    /// </summary>
    /// <value>
    /// The major version.
    /// </value>
    public int Major { get; private set; }

    /// <summary>
    /// Gets the minor version.
    /// </summary>
    /// <value>
    /// The minor version.
    /// </value>
    public int Minor { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginVersion"/> struct.
    /// </summary>
    /// <param name="major">The major version.</param>
    /// <param name="minor">The minor version.</param>
    public PluginVersion(int major, int minor)
      : this()
    {
      this.Major = major;
      this.Minor = minor;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginVersion"/> struct.
    /// </summary>
    /// <param name="version">The version.</param>
    /// <exception cref="System.ArgumentNullException">version</exception>
    /// <exception cref="System.ArgumentException">The version string must be on format intMajor.intMinor</exception>
    public PluginVersion(string version)
      : this()
    {
      if (version == null)
        throw new ArgumentNullException("version");

      try
      {
        string[] parts = version.Split(new char[] { '.' });
        if (parts.Length != 2)
          throw new FormatException();
        this.Major = int.Parse(parts[0], CultureInfo.InvariantCulture);
        this.Minor = int.Parse(parts[1], CultureInfo.InvariantCulture);
      }
      catch (FormatException)
      {
        throw new ArgumentException(Resources.VersionStringFormatError);
      }
    }

    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings:
    /// Value
    /// Meaning
    /// Less than zero
    /// This object is less than the <paramref name="other" /> parameter.
    /// Zero
    /// This object is equal to <paramref name="other" />.
    /// Greater than zero
    /// This object is greater than <paramref name="other" />.
    /// </returns>
    public int CompareTo(PluginVersion other)
    {
      if (this.Major != other.Major)
        return this.Major - other.Major;
      return this.Minor - other.Minor;
    }

    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="obj">An object to compare with this instance.</param>
    /// <returns>
    /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
    /// Value
    /// Meaning
    /// Less than zero
    /// This instance is less than <paramref name="obj" />.
    /// Zero
    /// This instance is equal to <paramref name="obj" />.
    /// Greater than zero
    /// This instance is greater than <paramref name="obj" />.
    /// </returns>
    int IComparable.CompareTo(object obj)
    {
      return this.CompareTo((PluginVersion)obj);
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
    /// </returns>
    public bool Equals(PluginVersion other)
    {
      return this.CompareTo(other) == 0;
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode()
    {
      return this.Major + this.Minor;
    }

    /// <summary>
    /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
    /// <returns>
    ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object obj)
    {
      if (!(obj is PluginVersion))
        return false;

      return this.Equals((PluginVersion)obj);
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", this.Major, this.Minor);
    }

    /// <summary>
    /// Returns true if left is less than right
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns><c>true</c> if left is less than right, <c>false</c> otherwise</returns>
    public static bool operator <(PluginVersion left, PluginVersion right)
    {
      return left.CompareTo(right) < 0;
    }

    /// <summary>
    /// Returns true if left is less than or equal to right
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns><c>true</c> if left is less than or equal to right, <c>false</c> otherwise</returns>
    public static bool operator <=(PluginVersion left, PluginVersion right)
    {
      return left.CompareTo(right) <= 0;
    }

    /// <summary>
    /// Returns true if left is greater than right
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns><c>true</c> if left is greater than right, <c>false</c> otherwise</returns>
    public static bool operator >(PluginVersion left, PluginVersion right)
    {
      return left.CompareTo(right) > 0;
    }

    /// <summary>
    /// Returns true if left is greater than or equal to right
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns><c>true</c> if left is greater than or equal to right, <c>false</c> otherwise</returns>
    public static bool operator >=(PluginVersion left, PluginVersion right)
    {
      return left.CompareTo(right) >= 0;
    }

    /// <summary>
    /// Returns true if left and right are equal
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns><c>true</c> if left and right are equal, <c>false</c> otherwise</returns>
    public static bool operator ==(PluginVersion left, PluginVersion right)
    {
      return left.Equals(right);
    }

    /// <summary>
    /// Returns true if left and right are not equal
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns><c>true</c> if left and right are not equal, <c>false</c> otherwise</returns>
    public static bool operator !=(PluginVersion left, PluginVersion right)
    {
      return !left.Equals(right);
    }
  }
}
