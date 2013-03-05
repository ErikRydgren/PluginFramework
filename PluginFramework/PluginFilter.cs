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
  using System.Linq;

  /// <summary>
  /// Static utility class for creating a <see cref="PluginFilter"/>.
  /// </summary>
  public class Plugin
  {
    public static PluginFilter Implements(Type interfacetype)
    {
      return Plugin.Implements(interfacetype.AssemblyQualifiedName);
    }

    public static PluginFilter Implements<T>()
    {
      return Plugin.Implements(typeof(T).AssemblyQualifiedName);
    }

    public static PluginFilter Implements(string qualifiedTypeName)
    {
      return new PluginFilter(PluginFilter.FilterOp.Implements, name: qualifiedTypeName);
    }

    public static PluginFilter DerivesFrom(Type interfacetype)
    {
      return Plugin.DerivesFrom(interfacetype.AssemblyQualifiedName);
    }

    public static PluginFilter DerivesFrom<T>()
    {
      return Plugin.DerivesFrom(typeof(T).AssemblyQualifiedName);
    }

    public static PluginFilter DerivesFrom(string qualifiedTypeName)
    {
      return new PluginFilter(PluginFilter.FilterOp.DerivesFrom, name: qualifiedTypeName);
    }

    public static PluginFilter IsNamed(string name)
    {
      return new PluginFilter(PluginFilter.FilterOp.IsNamed, name: name);
    }

    public static PluginFilter HasInfo(string key)
    {
      return new PluginFilter(PluginFilter.FilterOp.HasInfo, name: key);
    }

    public static PluginFilter InfoValue(string key, string value)
    {
      return new PluginFilter(PluginFilter.FilterOp.InfoValue, name: key + '=' + value);
    }

    public static PluginFilter Version(PluginVersion version)
    {
      return Plugin.MinVersion(version) & Plugin.MaxVersion(version);
    }

    public static PluginFilter MinVersion(PluginVersion version)
    {
      return new PluginFilter(PluginFilter.FilterOp.MinVersion, name: string.Format("{0}.{1}", version.Major, version.Minor));
    }

    public static PluginFilter MaxVersion(PluginVersion version)
    {
      return new PluginFilter(PluginFilter.FilterOp.MaxVersion, name: string.Format("{0}.{1}", version.Major, version.Minor));
    }
  }

  /// <summary>
  /// Describes the conditions a plugin needs to satisfy to be returned from a <see cref="IPluginRepository"/> search.
  /// </summary>
  [Serializable]
  public class PluginFilter
  {
    public enum FilterOp
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

    public FilterOp filterOp { get; private set; }
    public string name { get; private set; }
    public PluginFilter[] subFilters { get; private set; }

    public PluginFilter()
    {
    }

    internal PluginFilter(FilterOp filterOp, string name = null, PluginFilter[] subFilters = null)
    {
      this.filterOp = filterOp;
      this.name = name;
      this.subFilters = subFilters;
    }

    private static PluginFilter Combine(FilterOp op, PluginFilter left, PluginFilter right)
    {
      PluginFilter[] filters;

      if (left.filterOp == op && right.filterOp == op)
      {
        filters = new PluginFilter[left.subFilters.Length + right.subFilters.Length];
        left.subFilters.CopyTo(filters, 0);
        right.subFilters.CopyTo(filters, left.subFilters.Length);
      }
      else if (left.filterOp == op)
      {
        filters = new PluginFilter[left.subFilters.Length + 1];
        left.subFilters.CopyTo(filters, 0);
        filters[left.subFilters.Length] = right;
      }
      else if (right.filterOp == op)
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

    public static PluginFilter operator &(PluginFilter left, PluginFilter right)
    {
      return Combine(FilterOp.And, left, right);
    }

    public static PluginFilter operator |(PluginFilter left, PluginFilter right)
    {
      return Combine(FilterOp.Or, left, right);
    }

    public IEnumerable<PluginDescriptor> Filter(IEnumerable<PluginDescriptor> plugins)
    {
      switch (this.filterOp)
      {
        case FilterOp.Implements:
          {
            return plugins.Where(plugin => plugin.Interfaces.Contains(this.name));
          }

        case FilterOp.DerivesFrom:
          {
            return plugins.Where(plugin => plugin.Derives.Contains(this.name));
          }

        case FilterOp.IsNamed:
          {
            return plugins.Where(plugin => plugin.Name == this.name);
          }

        case FilterOp.HasInfo:
          {
            return plugins.Where(plugin => plugin.InfoValues.Keys.Contains(this.name));
          }

        case FilterOp.InfoValue:
          {
            string[] keyValue = this.name.Split("=".ToCharArray(), 2);
            return plugins.Where(plugin =>
            {
              string value;
              return plugin.InfoValues.TryGetValue(keyValue[0], out value) && value == keyValue[1];
            });
          }

        case FilterOp.MinVersion:
          {
            return plugins.Where(plugin => plugin.Version >= name);
          }

        case FilterOp.MaxVersion:
          {
            return plugins.Where(plugin => plugin.Version <= name);
          }

        case FilterOp.And:
          {
            foreach (var filter in this.subFilters)
              plugins = filter.Filter(plugins);

            return plugins;
          }

        case FilterOp.Or:
          {
            HashSet<PluginDescriptor> result = new HashSet<PluginDescriptor>();

            foreach (var filter in this.subFilters)
              foreach (var plugin in filter.Filter(plugins))
                result.Add(plugin);

            return result;
          }

        default:
          {
            throw new NotImplementedException(string.Format("Operator {0} not implemented yet", this.filterOp.ToString()));
          }
      }
    }

    public override string ToString()
    {
      switch (this.filterOp)
      {
        case FilterOp.Implements:
          return string.Format("Implements({0})", this.name);

        case FilterOp.DerivesFrom:
          return string.Format("DerivesFrom({0})", this.name);

        case FilterOp.IsNamed:
          return string.Format("Named({0})", this.name);

        case FilterOp.HasInfo:
          return string.Format("HasInfo({0})", this.name);

        case FilterOp.InfoValue:
          return string.Format("InfoValue({0})", this.name);

        case FilterOp.MinVersion:
          return string.Format("MinVersion({0})", this.name);

        case FilterOp.MaxVersion:
          return string.Format("MaxVersion({0})", this.name);

        case FilterOp.And:
          return string.Join(" & ", this.subFilters.Select(x => x.ToString()).ToArray());

        case FilterOp.Or:
          return string.Join(" | ", this.subFilters.Select(x => x.ToString()).ToArray());

        default:
          throw new NotImplementedException(string.Format("Operator {0} not implemented yet", this.filterOp.ToString()));
      }
    }
  }
}
