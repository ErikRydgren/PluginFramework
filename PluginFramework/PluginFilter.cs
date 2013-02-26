using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginFramework
{
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
            ISet<PluginDescriptor> result = new HashSet<PluginDescriptor>();

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

        case FilterOp.MinVersion:
          return string.Format("MinVersion({0})", this.name);

        case FilterOp.MaxVersion:
          return string.Format("MaxVersion({0})", this.name);

        case FilterOp.And:
          return string.Join(" & ", this.subFilters.Select(x => x.ToString()));

        case FilterOp.Or:
          return string.Join(" | ", this.subFilters.Select(x => x.ToString()));

        default:
          throw new NotImplementedException(string.Format("Operator {0} not implemented yet", this.filterOp.ToString()));
      }
    }
  }
}
