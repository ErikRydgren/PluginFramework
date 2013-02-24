using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Specialized;

namespace PluginFramework
{
  /// <summary>
  /// Implementation of <seealso cref="IPluginCreator"/>. Requires an IAssemblyRepository for fetching missing assemblies.
  /// </summary>
  public class PluginCreator : MarshalByRefObject, IPluginCreator
  {
    IAssemblyRepository repository;
    Dictionary<AppDomain, PluginCreator> creators = new Dictionary<AppDomain, PluginCreator>();

    /// <summary>
    /// Constructs a PluginCreator instance.
    /// </summary>
    /// <param name="repository">The <see cref="IAssemblyRepository"/> to use for resolving missing assemblies.</param>
    /// <remarks>Required</remarks>
    /// <exception cref="ArgumentNullException"/>
    public PluginCreator(IAssemblyRepository repository)
    {
      if (repository == null)
        throw new ArgumentNullException("repository");

      this.repository = repository;
    }

    /// <summary>
    /// Lookups or creates a PluginCreator located inside the target <see cref="AppDomain"/>.
    /// </summary>
    /// <param name="domain">The target <see cref="AppDomain"/></param>
    /// <returns>A PluginCreator instance running inside target <see cref="AppDomain"/></returns>
    /// <exception cref="PluginException"/>
    private PluginCreator GetCreatorFor(AppDomain domain)
    {
      try
      {
        PluginCreator creator;
        if (domain == null)
          creator = this;
        else if (domain == AppDomain.CurrentDomain)
          creator = this;
        else if (!creators.TryGetValue(domain, out creator))
        {
          creator = domain.CreateInstanceAndUnwrap(
            typeof(PluginCreator).Assembly.FullName,
            typeof(PluginCreator).FullName,
            false,
            BindingFlags.CreateInstance,
            null,
            new object[] { this.repository },
            null,
            null
          ) as PluginCreator;
          creators.Add(domain, creator);
        }

        return creator;
      }
      catch (PluginException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new PluginException(ex, ex.Message);
      }
    }

    /// <summary>
    /// Creates an instance of the plugin described by a <see cref="PluginDescriptor"/> inside a target <see cref="AppDomain"/> and then applying the provided settings.
    /// </summary>
    /// <typeparam name="T">The type the created plugin instance will returned as</typeparam>
    /// <param name="descriptor">A descriptor that identifies and describes the plugin to create</param>
    /// <param name="domain">The target <see cref="AppDomain"/>. If null then the instance will be created inside the current domain</param>
    /// <param name="settings">A key value storage with settings to apply to properties defined as PluginSettings on the created instance</param>
    /// <returns>The created plugin instance as T</returns>
    /// <exception cref="PluginException"/>
    public T Create<T>(PluginDescriptor descriptor, AppDomain domain = null, Dictionary<string, object> settings = null)
      where T : class
    {
      try
      {
        PluginCreator creator = this.GetCreatorFor(domain);

        object plugin = creator.Create(descriptor, settings);

        if (plugin == null)
          return null;

        if (!(plugin is T))
          throw new InvalidCastException(string.Format("{0} can't be casted to {1}", plugin.GetType().Name, typeof(T).Name));

        return plugin as T;
      }
      catch (PluginException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new PluginException(ex, ex.Message);
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
    private object Create(PluginDescriptor descriptor, Dictionary<string, object> settings)
    {
      AppDomain.CurrentDomain.AssemblyResolve += this.RepositoryResolve;
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
            throw new PluginException(ex, "Exception while applying setting {0} on {1}", name, descriptor.QualifiedName);
          }
        }
      }

      AppDomain.CurrentDomain.AssemblyResolve -= this.RepositoryResolve;
      return instance;
    }

    /// <summary>
    /// Handles assembly resolving for the AssemblyResolve event on the target <see cref="AppDomain"/> using the provided <see cref="IAssemblyResolver"/>
    /// </summary>
    /// <param name="sender">The target <see cref="AppDomain"/></param>
    /// <param name="e">The event arguments</param>
    /// <returns>A resolved assembly</returns>
    /// <exception cref="PluginException"/>
    private Assembly RepositoryResolve(object sender, ResolveEventArgs e)
    {
      // Try to reuse already loaded assembly
      Assembly[] currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
      for (int i = 0; i < currentAssemblies.Length; i++)
        if (currentAssemblies[i].FullName == e.Name)
          return currentAssemblies[i];

      // Fetch assembly from repository and load it into the appdomain
      byte[] assemblyBytes = this.repository.Get(e.Name);
      if (assemblyBytes != null)
        return Assembly.Load(assemblyBytes);

      // Unable to resolve this assembly
      throw new PluginException(string.Format("Unable to resolve assembly {0}", e.Name));
    }
  }
}
