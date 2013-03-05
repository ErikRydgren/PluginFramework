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
  using System.Reflection;

  /// <summary>
  /// Utility class for extracting plugins from assemblies provided from an IAssemblySource and exposing them as an IPluginSource
  /// </summary>
  public class AssemblySourceToPluginSource : IPluginSource
  {
    Dictionary<string, PluginDescriptor[]> assemblyPlugins;

    public AssemblySourceToPluginSource(IAssemblySource assemblySource)
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
            IList<CustomAttributeData> attributes = type.GetCustomAttributesData();

            PluginDescriptor plugin = new PluginDescriptor();
            pluginDescriptions[i] = plugin;

            plugin.QualifiedName = type.AssemblyQualifiedName;
            plugin.InfoValues = new Dictionary<string, string>();

            { // Info from PluginAttribute
              var attribute = attributes.First(x => x.Constructor.DeclaringType.FullName == typeof(PluginAttribute).FullName);
              var keyValues = attribute.NamedArguments.ToDictionary(x => x.MemberInfo.Name, x => x.TypedValue.Value);
              object value;
              if (keyValues.TryGetValue("Name", out value))
                plugin.Name = value as string;
            }

            { // Version from PluginVersionAttribute
              var attribute = attributes.FirstOrDefault(x => x.Constructor.DeclaringType.FullName == typeof(PluginVersionAttribute).FullName);
              if (attribute != null)
                plugin.Version = new PluginVersion((int)attribute.ConstructorArguments[0].Value, (int)attribute.ConstructorArguments[1].Value);
            }

            { // InfoValues from PluginInfoAttributes
              foreach (var attribute in attributes.Where(x => x.Constructor.DeclaringType.FullName == typeof(PluginInfoAttribute).FullName))
                plugin.InfoValues[attribute.ConstructorArguments[0].Value as string] = attribute.ConstructorArguments[1].Value as string;
            }

            { // Inherited ancestors
              List<Type> ancestors = new List<Type>();
              Type loop = type;
              while (loop.BaseType != null)
              {
                ancestors.Add(loop.BaseType);
                loop = loop.BaseType;
              }
              plugin.Derives = ancestors.Select(x => new QualifiedName(x.AssemblyQualifiedName)).ToArray();
            }

            { // Implemented interfaces
              plugin.Interfaces = type.GetInterfaces().Select(x => new QualifiedName(x.AssemblyQualifiedName)).ToArray();
            }

            { // Fetch plugin settings
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
                    setting.Name = (string)namedArg.TypedValue.Value;
                  else if (namedArg.MemberInfo.Name == "Required")
                    setting.Required = (bool)namedArg.TypedValue.Value;
                }
                if (setting.Name == null)
                  setting.Name = property.Name;
                setting.Type = property.PropertyType.AssemblyQualifiedName;

                settings.Add(setting);
              }
              plugin.settings = settings.ToArray();
            }
          }
          return pluginDescriptions;
        });

      this.OnAssemblyRemoved(sender, new AssemblyRemovedEventArgs(e.AssemblyId));

      if (plugins.Length > 0)
      {
        this.assemblyPlugins.Add(e.AssemblyId, plugins);
        if (this.PluginAdded != null)
          foreach (var plugin in plugins)
            this.PluginAdded(this, new PluginEventArgs(plugin));
      }
    }

    void OnAssemblyRemoved(object sender, AssemblyRemovedEventArgs e)
    {
      PluginDescriptor[] lostPlugins;
      if (this.assemblyPlugins.TryGetValue(e.AssemblyId, out lostPlugins))
      {
        if (this.PluginRemoved != null)
          foreach (var plugin in lostPlugins)
            this.PluginRemoved(this, new PluginEventArgs(plugin));

        this.assemblyPlugins.Remove(e.AssemblyId);
      }
    }

    public event EventHandler<PluginEventArgs> PluginAdded;
    public event EventHandler<PluginEventArgs> PluginRemoved;
  }
}
