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
  /// Utility class for managing fully assembly qualified type names
  /// </summary>
  [Serializable]
  public struct QualifiedName
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="QualifiedName"/> struct.
    /// </summary>
    /// <param name="qualifiedName">An assembly qualified typename</param>
    public QualifiedName(string qualifiedName)
      : this()
    {
      var parts = Split(qualifiedName);
      this.TypeFullName = parts[0];
      this.AssemblyFullName = parts[1];
    }

    /// <summary>
    /// Gets the full name of the type.
    /// </summary>
    /// <value>
    /// The full name of the type.
    /// </value>
    public string TypeFullName
    {
      get; private set; 
    }

    /// <summary>
    /// Gets the full name of the assembly.
    /// </summary>
    /// <value>
    /// The full name of the assembly.
    /// </value>
    public string AssemblyFullName
    {
      get; private set;
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      return this;
    }

    /// <summary>
    /// Splits the specified qualified name to typename and assembly name.
    /// </summary>
    /// <param name="qualifiedName">The assembly qualified tyoename</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">qualifiedName</exception>
    /// <exception cref="System.ArgumentException">qualifiedName must be a fully assembly qualified typename</exception>
    public static string[] Split(string qualifiedName)
    {
      if (qualifiedName == null)
        throw new ArgumentNullException("qualifiedName");

      string[] delimiters = new string[] { ", " };
      string[] nameParts = qualifiedName.Split(delimiters, 2, StringSplitOptions.None);

      if (nameParts.Length != 2)
        throw new ArgumentException("qualifiedName must be a fully assembly qualified typename");

      return nameParts;
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
      if (obj == null)
        return false;

      if (!(obj is QualifiedName))
        return false;

      QualifiedName other = (QualifiedName) obj;
      return this.TypeFullName.Equals(other.TypeFullName, StringComparison.OrdinalIgnoreCase) 
          && this.AssemblyFullName.Equals(other.AssemblyFullName, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode()
    {
      return this.TypeFullName.GetHashCode() & this.AssemblyFullName.GetHashCode();
    }

    public static implicit operator QualifiedName(string name)
    {
      return new QualifiedName(name);
    }

    public static implicit operator string(QualifiedName name)
    {
      return name.TypeFullName + ", " + name.AssemblyFullName;
    }

    public static bool operator == (QualifiedName left, QualifiedName right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(QualifiedName left, QualifiedName right)
    {
      return !left.Equals(right);
    }
  }
}
