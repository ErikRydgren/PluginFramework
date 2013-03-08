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

  /// <summary>
  /// Static utility class for creating a <see cref="PluginFilter"/>.
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

      return IsTypeOf(type.AssemblyQualifiedName);
    }

    /// <summary>
    /// Creates a filter that requires that plugins implement or derive from the named type.
    /// </summary>
    /// <param name="qualifiedTypeName">Qualified name of the type.</param>
    /// <returns>The created filter</returns>
    public static PluginFilter IsTypeOf(string qualifiedTypeName)
    {
      return Implements(qualifiedTypeName).Or(DerivesFrom(qualifiedTypeName));
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
      return new PluginFilter(PluginFilter.FilterOperation.DerivesFrom, operationData: baseType);
    }

    /// <summary>
    /// Creates a filter that requires that the name of plugins matches name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The created filter</returns>
    public static PluginFilter IsNamed(string name)
    {
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
    /// <param name="pluginVersion">The plugin version.</param>
    /// <returns>The created version</returns>
    public static PluginFilter HasVersion(string pluginVersion)
    {
      return Plugin.HasVersion((PluginVersion)pluginVersion);
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
    /// <param name="pluginVersion">The plugin version.</param>
    /// <returns>The created filter</returns>
    public static PluginFilter HasMinVersion(string pluginVersion)
    {
      return Plugin.HasMinVersion((PluginVersion)pluginVersion);
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
    /// <param name="pluginVersion">The plugin version.</param>
    /// <returns>The created filter</returns>
    public static PluginFilter HasMaxVersion(string pluginVersion)
    {
      return Plugin.HasMaxVersion((PluginVersion)pluginVersion);
    }
  }

  /// <summary>
  /// Describes the conditions a plugin needs to satisfy to be returned from a <see cref="IPluginRepository" /> search.
  /// </summary>
  [Serializable]
  public class PluginFilter
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
    internal FilterOperation operation { get; private set; }

    /// <summary>
    /// Gets the data needed to perform operation.
    /// </summary>
    /// <value>
    /// The operation data.
    /// </value>
    internal string operationData { get; private set; }

    /// <summary>
    /// Gets the sub filters for boolean operations.
    /// </summary>
    /// <value>
    /// The sub filters.
    /// </value>
    internal PluginFilter[] subFilters { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginFilter"/> class.
    /// </summary>
    public PluginFilter()
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
      this.operation = filterOp;
      this.operationData = operationData;
      this.subFilters = subFilters;
    }

    /// <summary>
    /// Creates a combined PluginFilter from two filters and a provided boolean operation.
    /// </summary>
    /// <param name="op">The boolean operation</param>
    /// <param name="left">The left filter</param>
    /// <param name="right">The right filter</param>
    /// <returns>The created filter</returns>
    private static PluginFilter Combine(FilterOperation op, PluginFilter left, PluginFilter right)
    {
      PluginFilter[] filters;

      if (left.operation == op && right.operation == op)
      {
        filters = new PluginFilter[left.subFilters.Length + right.subFilters.Length];
        left.subFilters.CopyTo(filters, 0);
        right.subFilters.CopyTo(filters, left.subFilters.Length);
      }
      else if (left.operation == op)
      {
        filters = new PluginFilter[left.subFilters.Length + 1];
        left.subFilters.CopyTo(filters, 0);
        filters[left.subFilters.Length] = right;
      }
      else if (right.operation == op)
      {
        filters = new PluginFilter[right.subFilters.Length + 1];
        filters[0] = left;
        right.subFilters.CopyTo(filters, 1);
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

      return this.And(Plugin.Implements(type).Or(Plugin.DerivesFrom(type)));
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also implement or derive from the named type.
    /// </summary>
    /// <param name="qualifiedTypeName">Qualified name of the type.</param>
    /// <returns>The created filter</returns>
    public PluginFilter IsTypeOf(string qualifiedTypeName)
    {
      return this.And(Plugin.Implements(qualifiedTypeName).Or(Plugin.DerivesFrom(qualifiedTypeName)));
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

      return this.And(Plugin.Implements(interfaceType));
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also implements the named interface.
    /// </summary>
    /// <param name="qualifiedTypeName">Name of the qualified type.</param>
    /// <returns>The created filter</returns>
    public PluginFilter Implements(string qualifiedTypeName)
    {
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

      return this.And(Plugin.DerivesFrom(baseType));
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also derive from the named type.
    /// </summary>
    /// <param name="baseType">Qualified name of the required base.</param>
    /// <returns>The created filter</returns>
    /// <exception cref="System.ArgumentNullException">baseType</exception>
    public PluginFilter DerivesFrom(string baseType)
    {
      return this.And(Plugin.DerivesFrom(baseType));
    }

    /// <summary>
    /// Creates a filter that requires plugins passes this filter and also that the name of plugins matches name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The created filter</returns>
    public PluginFilter IsNamed(string name)
    {
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
    public PluginFilter HasInfoValue(string key, string value)
    {
      return this.And(Plugin.HasInfoValue(key, value));
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also have a specified version.
    /// </summary>
    /// <param name="pluginVersion">The plugin version.</param>
    /// <returns>The created filter</returns>
    public PluginFilter HasVersion(PluginVersion pluginVersion)
    {
      return this.And(Plugin.HasVersion(pluginVersion));
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also have a specified version.
    /// </summary>
    /// <param name="pluginVersion">The plugin version.</param>
    /// <returns>The created filter</returns>
    public PluginFilter HasVersion(string pluginVersion)
    {
      return this.And(Plugin.HasVersion(pluginVersion));
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also have a version greater than or equal to the specified version.
    /// </summary>
    /// <param name="pluginVersion">The plugin version.</param>
    /// <returns>The created filter</returns>
    public PluginFilter HasMinVersion(PluginVersion pluginVersion)
    {
      return this.And(Plugin.HasMinVersion(pluginVersion));
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also have a version greater than or equal to the specified version.
    /// </summary>
    /// <param name="pluginVersion">The plugin version.</param>
    /// <returns>The created filter</returns>
    public PluginFilter HasMinVersion(string pluginVersion)
    {
      return this.And(Plugin.HasMinVersion(pluginVersion));
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also have a version less than or equal to the specified version.
    /// </summary>
    /// <param name="pluginVersion">The plugin version.</param>
    /// <returns>The created filter</returns>
    public PluginFilter HasMaxVersion(PluginVersion pluginVersion)
    {
      return this.And(Plugin.HasMaxVersion(pluginVersion));
    }

    /// <summary>
    /// Creates a filter that requires that plugins passes this filter and also have a version less than or equal to the specified version.
    /// </summary>
    /// <param name="pluginVersion">The plugin version.</param>
    /// <returns>The created filter</returns>
    public PluginFilter HasMaxVersion(string pluginVersion)
    {
      return this.And(Plugin.HasMaxVersion(pluginVersion));
    }

    /// <summary>
    /// Applies this filter to plugins.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>Enumeration of plugins that passes this filter</returns>
    /// <exception cref="System.NotImplementedException"></exception>
    internal IEnumerable<PluginDescriptor> Filter(IEnumerable<PluginDescriptor> plugins)
    {
      switch (this.operation)
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
          throw new NotImplementedException(string.Format(CultureInfo.InvariantCulture, "Operator {0} not implemented yet", this.operation.ToString()));
      }
    }

    /// <summary>
    /// Applies the max version filter.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>Enumeration of plugins that passes the filter</returns>
    private IEnumerable<PluginDescriptor> ApplyMaxVersionFilter(IEnumerable<PluginDescriptor> plugins)
    {
      return plugins.Where(plugin => plugin.Version <= (PluginVersion)operationData);
    }

    /// <summary>
    /// Applies the min version filter.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>Enumeration of plugins that passes the filter</returns>
    private IEnumerable<PluginDescriptor> ApplyMinVersionFilter(IEnumerable<PluginDescriptor> plugins)
    {
      return plugins.Where(plugin => plugin.Version >= (PluginVersion)operationData);
    }

    /// <summary>
    /// Applies the has info filter.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>Enumeration of plugins that passes the filter</returns>
    private IEnumerable<PluginDescriptor> ApplyHasInfoFilter(IEnumerable<PluginDescriptor> plugins)
    {
      return plugins.Where(plugin => plugin.InfoValues.Keys.Contains(this.operationData));
    }

    /// <summary>
    /// Applies the is named filter.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>Enumeration of plugins that passes the filter</returns>
    private IEnumerable<PluginDescriptor> ApplyIsNamedFilter(IEnumerable<PluginDescriptor> plugins)
    {
      return plugins.Where(plugin => plugin.Name == this.operationData);
    }

    /// <summary>
    /// Applies the derives from filter.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>Enumeration of plugins that passes the filter</returns>
    private IEnumerable<PluginDescriptor> ApplyDerivesFromFilter(IEnumerable<PluginDescriptor> plugins)
    {
      return plugins.Where(plugin => plugin.Derives.Contains(this.operationData));
    }

    /// <summary>
    /// Applies the implements filter.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>Enumeration of plugins that passes the filter</returns>
    private IEnumerable<PluginDescriptor> ApplyImplementsFilter(IEnumerable<PluginDescriptor> plugins)
    {
      return plugins.Where(plugin => plugin.Interfaces.Contains(this.operationData));
    }

    /// <summary>
    /// Applies the or filter.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>Enumeration of plugins that passes the filter</returns>
    private IEnumerable<PluginDescriptor> ApplyOrFilter(IEnumerable<PluginDescriptor> plugins)
    {
      HashSet<PluginDescriptor> result = new HashSet<PluginDescriptor>();

      foreach (var filter in this.subFilters)
        foreach (var plugin in filter.Filter(plugins))
          result.Add(plugin);

      return result;
    }

    /// <summary>
    /// Applies the and filter.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>Enumeration of plugins that passes the filter</returns>
    private IEnumerable<PluginDescriptor> ApplyAndFilter(IEnumerable<PluginDescriptor> plugins)
    {
      foreach (var filter in this.subFilters)
        plugins = filter.Filter(plugins);

      return plugins;
    }

    /// <summary>
    /// Applies the info value filter.
    /// </summary>
    /// <param name="plugins">The plugins.</param>
    /// <returns>Enumeration of plugins that passes the filter</returns>
    private IEnumerable<PluginDescriptor> ApplyInfoValueFilter(IEnumerable<PluginDescriptor> plugins)
    {
      string[] keyValue = this.operationData.Split("=".ToCharArray(), 2);
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
      switch (this.operation)
      {
        case FilterOperation.Implements:
          return "Implements(" + operationData + ")";

        case FilterOperation.DerivesFrom:
          return "DerivesFrom(" + this.operationData + ")";

        case FilterOperation.IsNamed:
          return "Named(" + this.operationData + ")";

        case FilterOperation.HasInfo:
          return "HasInfo(" + this.operationData + ")";

        case FilterOperation.InfoValue:
          return "InfoValue(" + this.operationData + ")";

        case FilterOperation.MinVersion:
          return "MinVersion(" + this.operationData + ")";

        case FilterOperation.MaxVersion:
          return "MaxVersion(" + this.operationData + ")";

        case FilterOperation.And:
          return string.Join(" & ", this.subFilters.Select(x => x.ToString()).ToArray());

        case FilterOperation.Or:
          return "( " + string.Join(" ) | ( ", this.subFilters.Select(x => x.ToString()).ToArray()) + " )";

        default:
          return this.operation.ToString() + "(" + this.operationData + ")";
      }
    }
  }
}
