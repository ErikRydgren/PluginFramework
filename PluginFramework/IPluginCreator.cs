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
  using System.Security.Permissions;

  /// <summary>
  /// Interface for creating instances of plugins described by <see cref="PluginDescriptor"/> inside a target <see cref="AppDomain"/>.
  /// </summary>
  public interface IPluginCreator
  {
    /// <summary>
    /// Creates an instance of the plugin described by the provided descriptor.
    /// </summary>
    /// <typeparam name="T">The type the plugin will be returned as.</typeparam>
    /// <param name="descriptor">The plugin descriptor.</param>
    /// <param name="assemblyRepository">The assembly repository used for resolving missing assemblies.</param>
    /// <param name="settings">The settings used to initialize the plugin.</param>
    /// <returns>The created plugin instance as T</returns>
    /// <exception cref="System.InvalidCastException"/>
    /// <exception cref="PluginFramework.PluginException"/>
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.ControlAppDomain)]
    T Create<T>(PluginDescriptor descriptor, IAssemblyRepository assemblyRepository, Dictionary<string, object> settings) where T : class;
  }

  public static class IPluginCreatorExtension
  {
    /// <summary>
    /// Creates an instance of the plugin described by the provided descriptor.
    /// </summary>
    /// <typeparam name="T">The type the plugin will be returned as.</typeparam>
    /// <param name="descriptor">The plugin descriptor.</param>
    /// <param name="assemblyRepository">The assembly repository used for resolving missing assemblies.</param>
    /// <param name="settings">The settings used to initialize the plugin.</param>
    /// <returns>The created plugin instance as T</returns>
    /// <exception cref="System.ArgumentNullException">creator</exception>
    /// <exception cref="System.InvalidCastException"/>
    /// <exception cref="PluginFramework.PluginException"/>
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.ControlAppDomain)]
    public static T Create<T>(this IPluginCreator creator, PluginDescriptor pluginDescriptor, IAssemblyRepository assemblyRepository) 
      where T : class
    {      
      if (creator == null)
        throw new ArgumentNullException("creator");

      return creator.Create<T>(pluginDescriptor, assemblyRepository, null);
    }
  }
}
