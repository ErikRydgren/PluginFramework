using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginFramework
{
  /// <summary>
  /// Provides methods for finding plugins with specified properties.
  /// </summary>
  public interface IPluginRepository
  {
    /// <summary>
    /// Method for querying for plugins that satisfies a supplied filter
    /// </summary>
    /// <param name="filter">The requirements that plugins must fulfill to be returned</param>
    /// <returns>Enumerable of plugins that fulfills the requirements</returns>
    IEnumerable<PluginDescriptor> Plugins(PluginFilter filter = null);
  }

  /// <summary>
  /// Utility functions that extends IPluginRepository with common query patterns  
  /// </summary>
  public static class IPluginRepositoryUtilityExtensions
  {
    /// <summary>
    /// Returns plugins that implements or derives from T and also fulfills the supplied requirements
    /// </summary>
    /// <typeparam name="T">The class that the plugin must derive from or the interface the plugin must implement</typeparam>
    /// <param name="repository">The <see cref="IPluginRepository"/> to query</param>
    /// <param name="filter">The extra requirements that plugins must fulfill to be returned</param>
    /// <returns>Enumerable of plugins that implements T or derives from T and that fulfills the extra requirements</returns>
    public static IEnumerable<PluginDescriptor> Plugins<T>(this IPluginRepository repository, PluginFilter filter = null)
    {
      PluginFilter combinedFilter = Plugin.Implements<T>() | Plugin.DerivesFrom<T>();
      if (filter != null)
        combinedFilter = combinedFilter & filter;
      return repository.Plugins(combinedFilter);
    }
  }
}
