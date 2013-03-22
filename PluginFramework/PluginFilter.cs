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
  using System.Linq;
  using System.Runtime.Serialization;
  using System.Security.Permissions;

  /// <summary>
  /// Static utility class for creating <see cref="PluginFilter"/>s.
  /// </summary>
  public static class Plugin
  {
    /// <summary>
    /// Creates a filter that requires that plugins implement or derive from the provided type.
    /// </summary>
    /// <param name="type">Type the plugin is required to implement or derive from</param>
    /// <returns>The created filter</returns>
    /// <exception cref="System.ArgumentNullException">type</exception>
    public static PluginFilter IsTypeOf(Type type)
    {
      if (type == null)
        throw new ArgumentNullException("type");

      return Plugin.IsTypeOf(type.AssemblyQualifiedName);
    }

    /// <summary>
    /// Creates a filter that requires that plugins implement or derive from the named type.
    /// </summary>
    /// <param name="qualifiedTypeName">Qualified name of the type.</param>
    /// <returns>The created filter</returns>
    public static PluginFilter IsTypeOf(string qualifiedTypeName)
    {
      if (qualifiedTypeName == null)
        throw new ArgumentNullException("qualifiedTypeName");

      return Plugin.Implements(qualifiedTypeName).Or(Plugin.DerivesFrom(qualifiedTypeName));
    }

    /// <summary>
    /// Creates a filter that requires that plugins implements the provided interface type.
    /// </summary>
    /// <param name="interfaceType">Type of the interface the plugin is required to implement.</param>
    /// <returns>The created filter</returns>
    /// <exception cref="System.ArgumentNullException">interfaceType</exception>
    public static PluginFilter Implements(Type interfaceType)
    {
      if (interfaceType == null)
        throw new ArgumentNullException("interfaceType");

      return Plugin.Implements(interfaceType.AssemblyQualifiedName);
    }

    /// <summary>
    /// Creates a filter that requires that plugins implements the named interface.
    /// </summary>
    /// <param name="qualifiedTypeName">Name of the qualified type.</param>
    /// <returns>The created filter</returns>
    public static PluginFilter Implements(string qualifiedTypeName)
    {
      if (qualifiedTypeName == null)
        throw new ArgumentNullException("qualifiedTypeName");

      return new PluginFilter(PluginFilter.FilterOperation.Implements, operationData: qualifiedTypeName);
    }

    /// <summary>
    /// Creates a filter that requires that plugins derive from the provided type.
    /// </summary>
    /// <param name="baseType">Type of the required base.</param>
    /// <returns>The created filter</returns>
    /// <exception cref="System.ArgumentNullException">baseType</exception>
    public static PluginFilter DerivesFrom(Type baseType)
    {
      if (baseType == null)
        throw new ArgumentNullException("baseType");

      return Plugin.DerivesFrom(baseType.AssemblyQualifiedName);
    }

    /// <summary>
    /// Creates a filter that requires that plugins derive from the named type.
    /// </summary>
    /// <param name="baseType">Qualified name of the required base.</param>
    /// <returns>The created filter</returns>
    /// <exception cref="System.ArgumentNullException">baseType</exception>
    public static PluginFilter DerivesFrom(string baseType)
    {
      if (baseType == null)
        throw new ArgumentNullException("baseType");

      return new PluginFilter(PluginFilter.FilterOperation.DerivesFrom, operationData: baseType);
    }

    /// <summary>
    /// Creates a filter that requires that the name of plugins matches name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The created filter</returns>
    public static PluginFilter IsNamed(string name)
    {
      if (name == null)
        throw new ArgumentNullException("name");

      return new PluginFilter(PluginFilter.FilterOperation.IsNamed, operationData: name);
    }

    /// <summary>
    /// Creates a filter that requires that plugins have a specified metainfo key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>
    /// The created filter
    /// </returns>
    public static PluginFilter HasInfo(string key)
    {
      if (key == null)
        throw new ArgumentNullException("key");

      return new PluginFilter(PluginFilter.FilterOperation.HasInfo, operationData: key);
    }

    /// <summary>
    /// Creates a filter that requires that plugins have a specified metainfo value.
    /// </summary>
    /// <param name="key">The metainfo key.</param>
    /// <param name="value">The metainfo value.</param>
    /// <returns>
    /// The created filter
    /// </returns>
    public static PluginFilter HasInfoValue(string key, string value)
    {
      if (key == null)
        throw new ArgumentNullException("key");

      if (value == null)
        throw new ArgumentNullException("value");

      return new PluginFilter(PluginFilter.FilterOperation.InfoValue, operationData: key + '=' + value);
    }

    /// <summary>
    /// Creates a filter that requires that plugins have a specified version.
    /// </summary>
    /// <param name="pluginVersion">The plugin version.</param>
    /// <returns>The created filter</returns>
    public static PluginFilter HasVersion(PluginVersion pluginVersion)
    {
      return Plugin.HasMinVersion(pluginVersion).HasMaxVersion(pluginVersion);
    }

    /// <summary>
    /// Creates a filter that requires that plugins have a specified version.
    /// </summary>
    /// <param name="version">The plugin version.</param>
    /// <returns>
    /// The created version
    /// </returns>
    /// <exception cref="System.ArgumentNullException">version</exception>
    public static PluginFilter HasVersion(string version)
    {
      if (version == null)
        throw new ArgumentNullException("version");

      return Plugin.HasVersion(new PluginVersion(version));
    }

    /// <summary>
    /// Creates a filter that requires that plugins have a version greater than or equal to the specified version.
    /// </summary>
    /// <param name="pluginVersion">The plugin version.</param>
    /// <returns>The created filter</returns>
    public static PluginFilter HasMinVersion(PluginVersion pluginVersion)
    {
      return new PluginFilter(PluginFilter.FilterOperation.MinVersion, operationData: string.Format(CultureInfo.InvariantCulture, "{0}.{1}", pluginVersion.Major, pluginVersion.Minor));
    }

    /// <summary>
    /// Creates a filter that requires that plugins have a version greater than or equal to the specified version.
    /// </summary>
    /// <param name="version">The plugin version.</param>
    /// <returns>
    /// The created filter
    /// </returns>
    /// <exception cref="System.ArgumentNullException">version</exception>
    public static PluginFilter HasMinVersion(string version)
    {
      if (version == null)
        throw new ArgumentNullException("version");

      return Plugin.HasMinVersion(new PluginVersion(version));
    }

    /// <summary>
    /// Creates a filter that requires that plugins have a version less than or equal to the specified version.
    /// </summary>
    /// <param name="pluginVersion">The plugin version.</param>
    /// <returns>The created filter</returns>
    public static PluginFilter HasMaxVersion(PluginVersion pluginVersion)
    {
      return new PluginFilter(PluginFilter.FilterOperation.MaxVersion, operationData: string.Format(CultureInfo.InvariantCulture, "{0}.{1}", pluginVersion.Major, pluginVersion.Minor));
    }

    /// <summary>
    /// Creates a filter that requires that plugins have a version less than or equal to the specified version.
    /// </summary>
    /// <param name="version">The plugin version.</param>
    /// <returns>
    /// The created filter
    /// </returns>
    /// <exception cref="System.ArgumentNullException">version</exception>
    public static PluginFilter HasMaxVersion(string version)
    {
      if (version == null)
        throw new ArgumentNullException("version");

      return Plugin.HasMaxVersion(new PluginVersion(version));
    }
  }

  /// <summary>
  /// Describes the conditions a plugin needs to satisfy to be returned from a <see cref="IPluginRepository" /> search.
  /// </summary>
  [Serializable]
  public sealed class PluginFilter : ISerializable, IEquatable<PluginFilter>
  {
    /// <summary>
    /// The possible filter operations
    /// </summary>
    internal enum FilterOperation
    {
      Implements,
      DerivesFrom,
      IsNamed,
      HasInfo,
      InfoValue,
      MinVersion,
      MaxVersion,
      And,
      Or,
    }

    /// <summary>
    /// Gets the filter operation.
    /// </summary>
    /// <value>
    /// The operation.
    /// </value>
    internal FilterOperation Operation { get; private set; }

    /// <summary>
    /// Gets the data needed to perform operation.
    /// </summary>
    /// <value>
    /// The operation data.
    /// </value>
    internal string OperationData { get; private set; }

    /// <summary>
    /// Gets the sub filters for boolean operations.
    /// </summary>
    /// <value>
    /// The sub filters.
    /// </value>
    internal PluginFilter[] SubFilters { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginFilter"/> class.
    /// </summary>
    internal PluginFilter()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginFilter"/> class.
    /// </summary>
    /// <param name="filterOp">The filter operatrion</param>
    /// <param name="operationData">The data needed for the operation</param>
    /// <param name="subFilters">The sub filters.</param>
    internal PluginFilter(FilterOperation filterOp, string operationData = null, PluginFilter[] subFilters = null)
    {
      this.Operation = filterOp;
      this.OperationData = operationData;
      this.SubFilters = subFilters;
    }

    /// <summary>
    /// Creates a combined PluginFilter from two filters and a provided boolean operation. 
    /// The code keeps the combined tree as flat as possible by merging equal operators together.
    /// </summary>
    /// <param name="op">The boolean operation</param>
    /// <param name="left">The left filter</param>
    /// <param name="right">The right filter</param>
    /// <returns>The created filter</returns>
    internal static PluginFilter Combine(FilterOperation op, PluginFilter left, PluginFilter right)
    {
      if (op != FilterOperation.And && op != FilterOperation.Or)
        throw new ArgumentException("op must be And or Or");

      if (left == null)
        throw new ArgumentNullException("left");

      if (right == null)
        throw new ArgumentNullException("right");

      PluginFilter[] filters;
      if (left.Operation == op && right.Operation == op)
      {
        filters = new PluginFilter[left.SubFilters.Length + right.SubFilters.Length];
        left.SubFilters.CopyTo(filters, 0);
        right.SubFilters.CopyTo(filters, left.SubFilters.Length);
      }
      else if (left.Operation == op)
      {
        filters = new PluginFilter[left.SubFilters.Length + 1];
        left.SubFilters.CopyTo(filters, 0);
        filters[left.SubFilters.Length] = right;
      }
      else if (right.Operation == op)
      {
        filters = new PluginFilter[right.SubFilters.Length + 1];
        filters[0] = left;
        right.SubFilters.CopyTo(filters, 1);
      }
      else
      {
        filters = new PluginFilter[2];
        filters[0] = left;
        filters[1] = right;
      }

      return new PluginFilter(op, subFilters: filters);
    }

    /// <summary>
    /// Creates a filter that requires that plugins first pass this filter then pass the supplied filter  
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns>The created filter</returns>
    /// <exception cref="System.ArgumentNullException">filter</exception>
    public PluginFilter And(PluginFilter filter)
    {
      if (filter == null)
        throw new ArgumentNullException("filter");

      return Combine(FilterOperation.And, this, filter);
    }

    /// <summary>
    /// Creates a filter that requires that plugins either pass this filter or pass the supplied filter  
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns>The created filter</returns>
    /// <exception cref="System.ArgumentNullException">filter</exception>
    public PluginFilter Or(PluginFilter filter)
    {
      if (filter == null)
        throw new ArgumentNullException("filter");

      return Combine(FilterOperation.Or, this, filter);
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also implement or derive from the provided type.
    /// </summary>
    /// <param name="type">Type the plugin is required to implement or derive from</param>
    /// <returns>The created filter</returns>
    /// <exception cref="System.ArgumentNullException">type</exception>
    public PluginFilter IsTypeOf(Type type)
    {
      if (type == null)
        throw new ArgumentNullException("type");

      return this.IsTypeOf(type.AssemblyQualifiedName);
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also implement or derive from the named type.
    /// </summary>
    /// <param name="qualifiedTypeName">Qualified name of the type.</param>
    /// <returns>The created filter</returns>
    public PluginFilter IsTypeOf(string qualifiedTypeName)
    {
      if (qualifiedTypeName == null)
        throw new ArgumentNullException("qualifiedTypeName");

      return this.And(Plugin.IsTypeOf(qualifiedTypeName));
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also implements the provided interface type.
    /// </summary>
    /// <param name="interfaceType">Type of the interface the plugin is required to implement.</param>
    /// <returns>The created filter</returns>
    /// <exception cref="System.ArgumentNullException">interfaceType</exception>
    public PluginFilter Implements(Type interfaceType)
    {
      if (interfaceType == null)
        throw new ArgumentNullException("interfaceType");

      return this.Implements(interfaceType.AssemblyQualifiedName);
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also implements the named interface.
    /// </summary>
    /// <param name="qualifiedTypeName">Name of the qualified type.</param>
    /// <returns>The created filter</returns>
    public PluginFilter Implements(string qualifiedTypeName)
    {
      if (qualifiedTypeName == null)
        throw new ArgumentNullException("qualifiedTypeName");

      return this.And(Plugin.Implements(qualifiedTypeName));
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also derive from the provided type.
    /// </summary>
    /// <param name="baseType">Type of the base.</param>
    /// <returns>
    /// The created filter
    /// </returns>
    /// <exception cref="System.ArgumentNullException">type</exception>
    public PluginFilter DerivesFrom(Type baseType)
    {
      if (baseType == null)
        throw new ArgumentNullException("baseType");

      return this.DerivesFrom(baseType.AssemblyQualifiedName);
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also derive from the named type.
    /// </summary>
    /// <param name="baseType">Qualified name of the required base.</param>
    /// <returns>The created filter</returns>
    /// <exception cref="System.ArgumentNullException">baseType</exception>
    public PluginFilter DerivesFrom(string baseType)
    {
      if (baseType == null)
        throw new ArgumentNullException("baseType");

      return this.And(Plugin.DerivesFrom(baseType));
    }

    /// <summary>
    /// Creates a filter that requires plugins passes this filter and also that the name of plugins matches name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The created filter</returns>
    public PluginFilter IsNamed(string name)
    {
      if (name == null)
        throw new ArgumentNullException("name");

      return this.And(Plugin.IsNamed(name));
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also have a specified metainfo key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>
    /// The created filter
    /// </returns>
    public PluginFilter HasInfo(string key)
    {
      if (key == null)
        throw new ArgumentNullException("key");

      return this.And(Plugin.HasInfo(key));
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also have a specified metainfo value.
    /// </summary>
    /// <param name="key">The metainfo key.</param>
    /// <param name="value">The metainfo value.</param>
    /// <returns>
    /// The created filter
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// key
    /// or
    /// value
    /// </exception>
    public PluginFilter HasInfoValue(string key, string value)
    {
      if (key == null)
        throw new ArgumentNullException("key");

      if (value == null)
        throw new ArgumentNullException("value");

      return this.And(Plugin.HasInfoValue(key, value));
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also have a specified version.
    /// </summary>
    /// <param name="version">The plugin version.</param>
    /// <returns>
    /// The created filter
    /// </returns>
    public PluginFilter HasVersion(PluginVersion version)
    {
      return this.And(Plugin.HasVersion(version));
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also have a specified version.
    /// </summary>
    /// <param name="version">The plugin version.</param>
    /// <returns>
    /// The created filter
    /// </returns>
    /// <exception cref="System.ArgumentNullException">version</exception>
    public PluginFilter HasVersion(string version)
    {
      if (version == null)
        throw new ArgumentNullException("version");

      return this.HasVersion(new PluginVersion(version));
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also have a version greater than or equal to the specified version.
    /// </summary>
    /// <param name="version">The plugin version.</param>
    /// <returns>
    /// The created filter
    /// </returns>
    public PluginFilter HasMinVersion(PluginVersion version)
    {
      return this.And(Plugin.HasMinVersion(version));
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also have a version greater than or equal to the specified version.
    /// </summary>
    /// <param name="version">The plugin version.</param>
    /// <returns>
    /// The created filter
    /// </returns>
    /// <exception cref="System.ArgumentNullException">version</exception>
    public PluginFilter HasMinVersion(string version)
    {
      if (version == null)
        throw new ArgumentNullException("version");

      return this.HasMinVersion(new PluginVersion(version));
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also have a version less than or equal to the specified version.
    /// </summary>
    /// <param name="version">The plugin version.</param>
    /// <returns>
    /// The created filter
    /// </returns>
    public PluginFilter HasMaxVersion(PluginVersion version)
    {
      return this.And(Plugin.HasMaxVersion(version));
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also have a version less than or equal to the specified version.
    /// </summary>
    /// <param name="version">The plugin version.</param>
    /// <returns>
    /// The created filter
    /// </returns>
    /// <exception cref="System.ArgumentNullException">version</exception>
    public PluginFilter HasMaxVersion(string version)
    {
      if (version == null)
        throw new ArgumentNullException("version");

      return this.HasMaxVersion(new PluginVersion(version));
    }

    /// <summary>
    /// Applies this filter to plugins.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>
    /// Enumeration of plugins that passes this filter
    /// </returns>
    /// <exception cref="System.ArgumentNullException">plugins</exception>
    /// <exception cref="System.NotImplementedException"></exception>
    internal IEnumerable<PluginDescriptor> Filter(IEnumerable<PluginDescriptor> plugins)
    {
      if (plugins == null)
        throw new ArgumentNullException("plugins");

      switch (this.Operation)
      {
        case FilterOperation.Implements:
          return ApplyImplementsFilter(plugins);

        case FilterOperation.DerivesFrom:
          return ApplyDerivesFromFilter(plugins);

        case FilterOperation.IsNamed:
          return ApplyIsNamedFilter(plugins);

        case FilterOperation.HasInfo:
          return ApplyHasInfoFilter(plugins);

        case FilterOperation.InfoValue:
          return ApplyInfoValueFilter(plugins);

        case FilterOperation.MinVersion:
          return ApplyMinVersionFilter(plugins);

        case FilterOperation.MaxVersion:
          return ApplyMaxVersionFilter(plugins);

        case FilterOperation.And:
          return ApplyAndFilter(plugins);

        case FilterOperation.Or:
          return ApplyOrFilter(plugins);

        default:
          throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, "Operator {0} not implemented yet", this.Operation.ToString()));
      }
    }

    /// <summary>
    /// Applies the max version filter.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>
    /// Enumeration of plugins that passes the filter
    /// </returns>
    /// <exception cref="System.ArgumentNullException">plugins</exception>
    internal IEnumerable<PluginDescriptor> ApplyMaxVersionFilter(IEnumerable<PluginDescriptor> plugins)
    {
      if (plugins == null)
        throw new ArgumentNullException("plugins");

      return plugins.Where(plugin => plugin.Version <= new PluginVersion(OperationData));
    }

    /// <summary>
    /// Applies the min version filter.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>
    /// Enumeration of plugins that passes the filter
    /// </returns>
    /// <exception cref="System.ArgumentNullException">plugins</exception>
    internal IEnumerable<PluginDescriptor> ApplyMinVersionFilter(IEnumerable<PluginDescriptor> plugins)
    {
      if (plugins == null)
        throw new ArgumentNullException("plugins");

      return plugins.Where(plugin => plugin.Version >= new PluginVersion(OperationData));
    }

    /// <summary>
    /// Applies the has info filter.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>
    /// Enumeration of plugins that passes the filter
    /// </returns>
    /// <exception cref="System.ArgumentNullException">plugins</exception>
    internal IEnumerable<PluginDescriptor> ApplyHasInfoFilter(IEnumerable<PluginDescriptor> plugins)
    {
      if (plugins == null)
        throw new ArgumentNullException("plugins");

      return plugins.Where(plugin => plugin.InfoValues.Keys.Contains(this.OperationData));
    }

    /// <summary>
    /// Applies the is named filter.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>
    /// Enumeration of plugins that passes the filter
    /// </returns>
    /// <exception cref="System.ArgumentNullException">plugins</exception>
    internal IEnumerable<PluginDescriptor> ApplyIsNamedFilter(IEnumerable<PluginDescriptor> plugins)
    {
      if (plugins == null)
        throw new ArgumentNullException("plugins");

      return plugins.Where(plugin => plugin.Name == this.OperationData);
    }

    /// <summary>
    /// Applies the derives from filter.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>
    /// Enumeration of plugins that passes the filter
    /// </returns>
    /// <exception cref="System.ArgumentNullException">plugins</exception>
    internal IEnumerable<PluginDescriptor> ApplyDerivesFromFilter(IEnumerable<PluginDescriptor> plugins)
    {
      if (plugins == null)
        throw new ArgumentNullException("plugins");

      return plugins.Where(plugin => plugin.Derives.Any(x => x.ToString() == this.OperationData));
    }

    /// <summary>
    /// Applies the implements filter.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>
    /// Enumeration of plugins that passes the filter
    /// </returns>
    /// <exception cref="System.ArgumentNullException">plugins</exception>
    internal IEnumerable<PluginDescriptor> ApplyImplementsFilter(IEnumerable<PluginDescriptor> plugins)
    {
      if (plugins == null)
        throw new ArgumentNullException("plugins");

      return plugins.Where(plugin => plugin.Interfaces.Any(x => x.ToString() == this.OperationData));
    }

    /// <summary>
    /// Applies the or filter.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>
    /// Enumeration of plugins that passes the filter
    /// </returns>
    /// <exception cref="System.ArgumentNullException">plugins</exception>
    internal IEnumerable<PluginDescriptor> ApplyOrFilter(IEnumerable<PluginDescriptor> plugins)
    {
      if (plugins == null)
        throw new ArgumentNullException("plugins");

      HashSet<PluginDescriptor> result = new HashSet<PluginDescriptor>();

      foreach (var filter in this.SubFilters)
        foreach (var plugin in filter.Filter(plugins))
          result.Add(plugin);

      return result;
    }

    /// <summary>
    /// Applies the and filter.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>
    /// Enumeration of plugins that passes the filter
    /// </returns>
    /// <exception cref="System.ArgumentNullException">plugins</exception>
    internal IEnumerable<PluginDescriptor> ApplyAndFilter(IEnumerable<PluginDescriptor> plugins)
    {
      if (plugins == null)
        throw new ArgumentNullException("plugins");

      foreach (var filter in this.SubFilters)
        plugins = filter.Filter(plugins);

      return plugins;
    }

    /// <summary>
    /// Applies the info value filter.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>
    /// Enumeration of plugins that passes the filter
    /// </returns>
    /// <exception cref="System.ArgumentNullException">plugins</exception>    
    internal IEnumerable<PluginDescriptor> ApplyInfoValueFilter(IEnumerable<PluginDescriptor> plugins)
    {
      if (plugins == null)
        throw new ArgumentNullException("plugins");

      string[] keyValue = this.OperationData.Split("=".ToCharArray(), 2);
      return plugins.Where(plugin =>
      {
        string value;
        return plugin.InfoValues.TryGetValue(keyValue[0], out value) && value == keyValue[1];
      });
    }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
      switch (this.Operation)
      {
        case FilterOperation.And:
          return "(" + string.Join(" & ", this.SubFilters.Select(x => x.ToString()).ToArray()) + ")";

        case FilterOperation.Or:
          return "(" + string.Join(" | ", this.SubFilters.Select(x => x.ToString()).ToArray()) + ")";

        default:
          return this.Operation.ToString() + "(" + OperationData + ")";
      }
    }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
    /// </returns>
    public bool Equals(PluginFilter other)
    {
      if (other == null)
        return false;

      if (other.Operation != this.Operation)
        return false;

      if (other.OperationData != this.OperationData)
        return false;

      if (other.SubFilters != null)
      {
        return Enumerable.SequenceEqual(other.SubFilters, this.SubFilters);
      }
      
      return true;
    }

    #region Serialization
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
        throw new ArgumentNullException("info");

      info.AddValue("Operation", this.Operation.ToString());
      info.AddValue("OperationData", this.OperationData);
      
      if (this.SubFilters != null)
      {
        info.AddValue("SubFilters", this.SubFilters.Length);
        for (int i = 0; i < this.SubFilters.Length; i++)
          info.AddValue("SubFilter_" + i.ToString(CultureInfo.InvariantCulture), this.SubFilters[i]);
      }
      else
        info.AddValue("SubFilters", 0);
    }

    internal PluginFilter(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
        throw new ArgumentNullException("info");

      this.Operation = (FilterOperation)Enum.Parse(typeof(FilterOperation), (string)info.GetValue("Operation", typeof(string)));
      this.OperationData = info.GetString("OperationData");

      int subFilters = (int)info.GetValue("SubFilters", typeof(int));
      this.SubFilters = 
        Enumerable.Range(0, subFilters)
                  .Select(i => (PluginFilter)info.GetValue("SubFilter_" + i.ToString(CultureInfo.InvariantCulture), typeof(PluginFilter)))
                  .ToArray();
    }
    #endregion
  }
}
