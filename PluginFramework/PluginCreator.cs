using PluginFramework.Logging;
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
  using PluginFramework.Logging;
  using System.Security;

  /// <summary>
  /// Implementation of <see cref="IPluginCreator"/>.
  /// </summary>
  public class PluginCreator : MarshalByRefObject, IPluginCreator, ILogWriter
  {
    const string PLUGINCREATORKEY = "PluginFramework.PluginCreator";
    const string LOGGERFACTORYKEY = "PluginFramework.ILoggerFactory";

    ILog log;
    ILog ILogWriter.Log
    {
      get { return log; }
      set { log = value; }
    }

    /// <summary>
    /// External creation of instances not allowed. Use GetCreator to fetch a creator instance
    /// </summary>
    private PluginCreator()
    {
      this.InitLog();
      this.log.Info(Resources.CreatedInsideAppDomain, AppDomain.CurrentDomain.FriendlyName);
    }

    /// <summary>
    /// Fetches or Creates a IPluginCreator located inside the current <see cref="AppDomain"/>.
    /// </summary>
    /// <returns>A IPluginCreator instance running inside target <see cref="AppDomain"/></returns>
    /// <exception cref="PluginException"/>    
    [SecurityPermissionAttribute(SecurityAction.LinkDemand, ControlAppDomain = true)]
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
    [SecurityPermissionAttribute(SecurityAction.LinkDemand, ControlAppDomain = true)]
    public static IPluginCreator GetCreator(AppDomain domain)
    {
      return GetCreator(domain, Logger.Singleton.LoggerFactory);
    }

    [SecurityPermissionAttribute(SecurityAction.LinkDemand, ControlAppDomain = true)]
    internal static IPluginCreator GetCreator(AppDomain domain, ILoggerFactory logfactory)
    {
      if (domain == null)
        throw new ArgumentNullException("domain");

      if (logfactory == null)
        throw new ArgumentNullException("logfactory");

      IPluginCreator creator = domain.GetData(PLUGINCREATORKEY) as IPluginCreator;
      if (creator == null)
      {
        domain.SetData(LOGGERFACTORYKEY, new ProxyLoggerFactory(logfactory));
        domain.DoCallBack(() =>
        {
          Logger.Singleton.LoggerFactory = AppDomain.CurrentDomain.GetData(LOGGERFACTORYKEY) as ILoggerFactory;
          AppDomain.CurrentDomain.SetData(PLUGINCREATORKEY, new PluginCreator());
        });
        domain.SetData(LOGGERFACTORYKEY, null);
        creator = domain.GetData(PLUGINCREATORKEY) as IPluginCreator;
      }
      return creator;
    }

    /// <summary>
    /// Creates an instance of the plugin described by a <see cref="PluginDescriptor" /> inside a target <see cref="AppDomain" /> and then applying the provided settings.
    /// </summary>
    /// <param name="descriptor">A descriptor that identifies and describes the plugin to create. Required.</param>
    /// <param name="assemblies">The assembly repository used for resolving missing assemblies. Required.</param>
    /// <param name="settings">A key value storage with settings to apply to properties defined as PluginSettings on the created instance. Not required.</param>
    /// <returns>
    /// The created plugin
    /// </returns>
    /// <exception cref="PluginException"></exception>
    [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.ControlAppDomain)]
    public object Create(PluginDescriptor descriptor, IAssemblyRepository assemblies, IDictionary<string, object> settings)
    {
      if (descriptor == null)
        throw new ArgumentNullException("descriptor");

      if (assemblies == null)
        throw new ArgumentNullException("assemblies");

      try
      {
        ResolveEventHandler resolveHandler = new ResolveEventHandler((s, e) =>
          {
            // Fetch assembly from repository and load it into the appdomain
            byte[] assemblyBytes = assemblies.Fetch(e.Name);
            if (assemblyBytes != null)
              return Assembly.Load(assemblyBytes);

            // Unable to resolve this assembly
            return null;
          });

        AppDomain.CurrentDomain.AssemblyResolve += resolveHandler;        
        object plugin = AppDomain.CurrentDomain.CreateInstanceAndUnwrap(descriptor.QualifiedName.AssemblyFullName, descriptor.QualifiedName.TypeFullName);
        AppDomain.CurrentDomain.AssemblyResolve -= resolveHandler;

        ApplySettings(plugin, settings);

        this.log.Info(Resources.CreatedPlugin, plugin.GetType().FullName);

        return plugin;
      }
      catch (PluginException ex)
      {
        this.log.Error(ex.Message);
        throw;
      }
      catch (Exception ex)
      {
        this.log.Error(ex.Message);
        throw new PluginException(ex.Message, ex);
      }
    }

    /// <summary>
    /// Applies settings to a plugin instance.
    /// </summary>
    /// <param name="instance">The plugin instance to apply settings to</param>
    /// <param name="settings">A key value storage with settings to apply to propertied defined as PluginSettings on the created instance</param>
    /// <exception cref="PluginFramework.PluginSettingException">
    /// Required setting {0} not supplied while creating {1}
    /// or
    /// </exception>
    /// <exception cref="PluginException"></exception>
    /// <remarks>
    /// This function is run inside the target <see cref="AppDomain" />
    /// </remarks>
    private static void ApplySettings(object instance, IDictionary<string, object> settings)
    {
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
          throw new PluginSettingException(Resources.RequiredSettingNotSuppliedOnCreate, name, instance.GetType().AssemblyQualifiedName);

        if (setting != null)
        {
          try
          {
            property.SetValue(instance, setting, null);
          }
          catch (Exception ex)
          {
            throw new PluginSettingException(string.Format(CultureInfo.InvariantCulture, "Exception while applying setting {0} on {1}", name, instance.GetType().AssemblyQualifiedName), ex);
          }
        }
      }
    }
  }
}
