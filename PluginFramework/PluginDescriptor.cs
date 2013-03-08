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
  using System.Collections.Generic;
  using System.Globalization;

  /// <summary>
  /// Describes a plugin. Used by a <see cref="IPluginCreator"/> to create instances of the plugin.
  /// </summary>
  [Serializable]
  public class PluginDescriptor : IEquatable<PluginDescriptor>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginDescriptor"/> class.
    /// </summary>
    public PluginDescriptor()
    {
      this.Derives = new List<QualifiedName>();
      this.Interfaces = new List<QualifiedName>();
      this.InfoValues = new Dictionary<string, string>();
      this.Settings = new List<PluginSettingDescriptor>();
    }

    /// <summary>
    /// Gets or sets the fulle assembly qualified name of the plugin class.
    /// </summary>
    /// <value>
    /// The qualified name of the plugin class.
    /// </value>
    public QualifiedName QualifiedName { get; set; }

    /// <summary>
    /// Gets the classes the plugin inherits as a list of assembly qualified names.
    /// </summary>
    /// <value>
    /// The classnames the plugin inherits.
    /// </value>
    public IList<QualifiedName> Derives { get; private set; }

    /// <summary>
    /// Gets the interfaces the plugin implements as a list of assembly qualified names.
    /// </summary>
    /// <value>
    /// The interfaces.
    /// </value>
    public IList<QualifiedName> Interfaces { get; private set; }

    /// <summary>
    /// Gets the settings the plugin defines.
    /// </summary>
    /// <value>
    /// The plugin settings.
    /// </value>
    public IList<PluginSettingDescriptor> Settings { get; private set; }

    /// <summary>
    /// Gets the plugin name.
    /// </summary>
    /// <value>
    /// The plugin name.
    /// </value>
    public string Name { get; internal set; }

    /// <summary>
    /// Gets the plugin version.
    /// </summary>
    /// <value>
    /// The plugin version.
    /// </value>
    public PluginVersion Version { get; internal set; }

    /// <summary>
    /// Gets the plugin metainfo.
    /// </summary>
    /// <value>
    /// The plugin metainfo.
    /// </value>
    public IDictionary<string, string> InfoValues { get; private set; }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
    /// </returns>
    public bool Equals(PluginDescriptor other)
    {
      if (other == null)
        return false;

      return this.QualifiedName == other.QualifiedName;
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
      PluginDescriptor other = obj as PluginDescriptor;

      if (other == null)
        return false;

      return base.Equals(other);
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode()
    {
      return this.QualifiedName.GetHashCode();
    }
  }
}
