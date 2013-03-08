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
  using System.Reflection;
  using System.Security.Permissions;

  /// <summary>
  /// Implementation of <see cref="IPluginCreator"/>.
  /// </summary>
  public class PluginCreator : MarshalByRefObject, IPluginCreator
  {
    const string IDKEY = "PluginFramework.PluginCreator";

    /// <summary>
    /// External creation of instances not allowed. Use GetCreator to fetch a creator instance
    /// </summary>
    private PluginCreator()
    {
    }

    /// <summary>
    /// Fetches or Creates a IPluginCreator located inside the current <see cref="AppDomain"/>.
    /// </summary>
    /// <returns>A IPluginCreator instance running inside target <see cref="AppDomain"/></returns>
    /// <exception cref="PluginException"/>
    public static IPluginCreator GetCreator()
    {
      return GetCreator(AppDomain.CurrentDomain);
    }

    /// <summary>
    /// Fetches or Creates a IPluginCreator located inside the target <see cref="AppDomain"/>.
    /// </summary>
    /// <param name="domain">The target <see cref="AppDomain"/></param>
    /// <returns>A IPluginCreator instance running inside target <see cref="AppDomain"/></returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="PluginException"/>
    public static IPluginCreator GetCreator(AppDomain domain)
    {
      if (domain == null)
        throw new ArgumentNullException("domain");

      try
      {
        IPluginCreator creator = domain.GetData(IDKEY) as IPluginCreator;
        if (creator == null)
        {
          domain.DoCallBack(() => AppDomain.CurrentDomain.SetData(IDKEY, new PluginCreator()));
          creator = domain.GetData(IDKEY) as IPluginCreator;
        }
        return creator;
      }        
      catch (PluginException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new PluginException(ex.Message, ex);
      }
    }

    /// <summary>
    /// Creates an instance of the plugin described by a <see cref="PluginDescriptor" /> inside a target <see cref="AppDomain" /> and then applying the provided settings.
    /// </summary>
    /// <typeparam name="T">The type the created plugin instance will returned as</typeparam>
    /// <param name="descriptor">A descriptor that identifies and describes the plugin to create</param>
    /// <param name="assemblyRepository">The assembly repository used for resolving missing assemblies.</param>
    /// <param name="settings">A key value storage with settings to apply to properties defined as PluginSettings on the created instance</param>
    /// <returns>
    /// The created plugin instance as T
    /// </returns>
    /// <exception cref="PluginException"></exception>
    [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.ControlAppDomain)]
    public T Create<T>(PluginDescriptor descriptor, IAssemblyRepository assemblyRepository, Dictionary<string, object> settings)
      where T : class
    {
      try
      {
        ResolveEventHandler resolveHandler = new ResolveEventHandler((s, e) =>
          {
            // Fetch assembly from repository and load it into the appdomain
            byte[] assemblyBytes = assemblyRepository.Fetch(e.Name);
            if (assemblyBytes != null)
              return Assembly.Load(assemblyBytes);

            // Unable to resolve this assembly
            throw new PluginException("Unable to resolve assembly " + e.Name);
          });

        AppDomain.CurrentDomain.AssemblyResolve += resolveHandler;

        object plugin = this.Create(descriptor, settings);

        AppDomain.CurrentDomain.AssemblyResolve -= resolveHandler;

        return (T)plugin;
      }
      catch (PluginException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new PluginException(ex.Message, ex);
      }
    }

    /// <summary>
    /// Creates an instance of a plugin described by a descriptor. 
    /// </summary>
    /// <remarks>This function is run inside the target <see cref="AppDomain"/></remarks>
    /// <param name="descriptor">Descriptor of the plugin to create</param>
    /// <param name="settings">A key value storage with settings to apply to propertied defined as PluginSettings on the created instance</param>
    /// <returns>The created plugin instance</returns>
    /// <exception cref="PluginException"/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")] // Can't be static. We need an MarshalByRefObject instance to get into the target AppDomain.
    private object Create(PluginDescriptor descriptor, Dictionary<string, object> settings)
    {
      object instance = AppDomain.CurrentDomain.CreateInstanceAndUnwrap(descriptor.QualifiedName.AssemblyFullName, descriptor.QualifiedName.TypeFullName);

      foreach (var property in instance.GetType().GetProperties())
      {
        PluginSettingAttribute pluginSetting = property.GetCustomAttributes(typeof(PluginSettingAttribute), true).OfType<PluginSettingAttribute>().FirstOrDefault();
        if (pluginSetting == null)
          continue;

        string name = pluginSetting.Name ?? property.Name;
        object setting = null;
        if (settings != null)
          settings.TryGetValue(name, out setting);

        if (setting == null && pluginSetting.Required)
          throw new PluginException("Required setting {0} not supplied while creating {1}", name, descriptor.QualifiedName);

        if (setting != null)
        {
          try
          {
            property.SetValue(instance, setting, null);
          }
          catch (Exception ex)
          {
            throw new PluginException(string.Format(CultureInfo.InvariantCulture, "Exception while applying setting {0} on {1}", name, descriptor.QualifiedName), ex);
          }
        }
      }

      return instance;
    }
  }
}
