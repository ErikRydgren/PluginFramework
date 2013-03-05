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
  using System.Collections.Generic;

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
