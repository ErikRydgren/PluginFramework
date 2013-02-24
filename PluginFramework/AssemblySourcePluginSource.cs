using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace PluginFramework
{
  /// <summary>
  /// Utility class for extracting plugins from assemblies provided from an IAssemblySource and exposing them as an IPluginSource
  /// </summary>
  public class AssemblySourcePluginSource : IPluginSource
  {
    Dictionary<string, PluginDescriptor[]> assemblyPlugins;

    public AssemblySourcePluginSource(IAssemblySource assemblySource)
    {
      this.assemblyPlugins = new Dictionary<string, PluginDescriptor[]>();
      assemblySource.AssemblyAdded += new EventHandler<AssemblyAddedEventArgs>(OnAssemblyAdded);
      assemblySource.AssemblyRemoved += new EventHandler<AssemblyRemovedEventArgs>(OnAssemblyRemoved);
    }

    void OnAssemblyAdded(object sender, AssemblyAddedEventArgs e)
    {
      var plugins = e.Reflect(assembly =>
        {
          var pluginTypes =
            (from type in assembly.GetExportedTypes()
             where type.IsClass && !type.IsGenericType && type.IsPublic && type.IsVisible && !type.IsAbstract
             where type.GetCustomAttributesData().Any(x => x.Constructor.DeclaringType.FullName == typeof(PluginAttribute).FullName)
             select type).ToArray();

          PluginDescriptor[] pluginDescriptions = new PluginDescriptor[pluginTypes.Length];

          for (int i = 0; i < pluginTypes.Length; i++)
          {
            Type type = pluginTypes[i];
            object value;

            // Fetch named arguments for the PluginAttribute
            Dictionary<string, object> args = new Dictionary<string, object>();
            CustomAttributeData pluginAttr = type.GetCustomAttributesData().First(x => x.Constructor.DeclaringType.FullName == typeof(PluginAttribute).FullName);
            foreach (var namedArg in pluginAttr.NamedArguments)
            {
              string name = namedArg.MemberInfo.Name;
              args.Add(name, namedArg.TypedValue.Value);
            }

            // Fetch plugin version
            PluginVersion version = new PluginVersion();
            CustomAttributeData versionAttr = type.GetCustomAttributesData().FirstOrDefault(x => x.Constructor.DeclaringType.FullName == typeof(PluginVersionAttribute).FullName);
            if (versionAttr != null)
            {
              version.Major = (int)versionAttr.ConstructorArguments[0].Value;
              version.Minor = (int)versionAttr.ConstructorArguments[1].Value;
            }

            // Fetch inherited ancestors
            List<Type> ancestors = new List<Type>();
            Type loop = type;
            while (loop.BaseType != null)
            {
              ancestors.Add(loop.BaseType);
              loop = loop.BaseType;
            }

            // Fetch plugin settings
            List<PluginSettingDescriptor> settings = new List<PluginSettingDescriptor>();
            foreach (var property in type.GetProperties())
            {
              CustomAttributeData attribute = property.GetCustomAttributesData().FirstOrDefault(x => x.Constructor.DeclaringType.FullName == typeof(PluginSettingAttribute).FullName);
              if (attribute == null)
                continue;

              PluginSettingDescriptor setting = new PluginSettingDescriptor();
              foreach (var namedArg in attribute.NamedArguments)
              {
                if (namedArg.MemberInfo.Name == "Name")
                  setting.Name = (string) namedArg.TypedValue.Value;
                else if (namedArg.MemberInfo.Name == "Required")
                  setting.Required = (bool) namedArg.TypedValue.Value;
              }
              if (setting.Name == null)
                setting.Name = property.Name;
              setting.Type = property.PropertyType.AssemblyQualifiedName;

              settings.Add(setting);
            }

            // Create and return descriptor
            PluginDescriptor plugin = new PluginDescriptor();
            plugin.QualifiedName = type.AssemblyQualifiedName;
            plugin.Derives = ancestors.Select(x => new QualifiedName(x.AssemblyQualifiedName)).ToArray();
            plugin.Interfaces = type.GetInterfaces().Select(x => new QualifiedName(x.AssemblyQualifiedName)).ToArray();
            plugin.Name = args.TryGetValue("Name", out value) ? value.ToString() : type.Name;
            plugin.Version = version;
            plugin.settings = settings.ToArray();
            pluginDescriptions[i] = plugin;
          }
          return pluginDescriptions;
        });

      this.OnAssemblyRemoved(sender, new AssemblyRemovedEventArgs(e.AssemblyFullName));

      if (plugins.Length > 0)
      {
        this.assemblyPlugins.Add(e.AssemblyFullName, plugins);
        if (this.PluginAdded != null)
          foreach (var plugin in plugins)
            this.PluginAdded(this, new PluginEventArgs(plugin));
      }

    }

    void OnAssemblyRemoved(object sender, AssemblyRemovedEventArgs e)
    {
      PluginDescriptor[] lostPlugins;
      if (this.assemblyPlugins.TryGetValue(e.AssemblyFullName, out lostPlugins))
      {
        if (this.PluginRemoved != null)
          foreach (var plugin in lostPlugins)
            this.PluginRemoved(this, new PluginEventArgs(plugin));

        this.assemblyPlugins.Remove(e.AssemblyFullName);
      }
    }

    public event EventHandler<PluginEventArgs> PluginAdded;
    public event EventHandler<PluginEventArgs> PluginRemoved;
  }
}
