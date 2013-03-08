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
  /// Utility class for extracting plugins from assemblies provided from an IAssemblySource and exposing them through IPluginSource events
  /// </summary>
  public class AssemblySourceToPluginSource : IPluginSource
  {
    Dictionary<string, PluginDescriptor[]> assemblyPlugins;

    public AssemblySourceToPluginSource(IAssemblySource assemblySource)
    {
      if (assemblySource == null)
        throw new ArgumentNullException("assemblySource");

      this.assemblyPlugins = new Dictionary<string, PluginDescriptor[]>();
      assemblySource.AssemblyAdded += new EventHandler<AssemblyAddedEventArgs>(OnAssemblyAdded);
      assemblySource.AssemblyRemoved += new EventHandler<AssemblyRemovedEventArgs>(OnAssemblyRemoved);
    }

    private void OnAssemblyAdded(object sender, AssemblyAddedEventArgs e)
    {
      var plugins = e.Reflect(assembly =>
        {
          var pluginTypes =
            (from type in assembly.GetExportedTypes()
             where type.IsClass && !type.IsGenericType && type.IsPublic && type.IsVisible && !type.IsAbstract
             where CustomAttributeData.GetCustomAttributes(type).Any(x => x.Constructor.DeclaringType.FullName == typeof(PluginAttribute).FullName)
             select type).ToArray();

          PluginDescriptor[] pluginDescriptions = new PluginDescriptor[pluginTypes.Length];

          for (int i = 0; i < pluginTypes.Length; i++)
          {
            Type type = pluginTypes[i];
            IList<CustomAttributeData> attributes = CustomAttributeData.GetCustomAttributes(type);

            PluginDescriptor plugin = pluginDescriptions[i] = new PluginDescriptor();
            plugin.QualifiedName = type.AssemblyQualifiedName;
            plugin.Version = GetPluginVersion(attributes);

            SetPluginInfoValuesFromAttributes(plugin, attributes);
            SetPluginAncestors(plugin, type);
            SetPluginInterfaces(plugin, type);
            SetPluginSettings(plugin, type);

            var keyValues = GetPluginAttributeNamedValues(attributes);
            object value;
            if (keyValues.TryGetValue("Name", out value))
              plugin.Name = value as string;
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

    private void OnAssemblyRemoved(object sender, AssemblyRemovedEventArgs e)
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

    private static IDictionary<string, object> GetPluginAttributeNamedValues(IList<CustomAttributeData> attributes)
    {
      var attribute = attributes.First(x => x.Constructor.DeclaringType.FullName == typeof(PluginAttribute).FullName);
      return attribute.NamedArguments.ToDictionary(x => x.MemberInfo.Name, x => x.TypedValue.Value);
    }

    private static PluginVersion GetPluginVersion(IList<CustomAttributeData> attributes)
    { // Version from PluginVersionAttribute
      var attribute = attributes.FirstOrDefault(x => x.Constructor.DeclaringType.FullName == typeof(PluginVersionAttribute).FullName);
      return attribute != null ?
        new PluginVersion((int)attribute.ConstructorArguments[0].Value, (int)attribute.ConstructorArguments[1].Value) :
        new PluginVersion();
    }

    private static void SetPluginInfoValuesFromAttributes(PluginDescriptor plugin, IList<CustomAttributeData> attributes)
    {
      foreach (var attribute in attributes.Where(x => x.Constructor.DeclaringType.FullName == typeof(PluginInfoAttribute).FullName))
        plugin.InfoValues[attribute.ConstructorArguments[0].Value as string] = attribute.ConstructorArguments[1].Value as string;
    }

    private static void SetPluginAncestors(PluginDescriptor plugin, Type type)
    {
      Type loop = type;
      while (loop.BaseType != null)
      {
        plugin.Derives.Add(new QualifiedName(loop.BaseType.AssemblyQualifiedName));
        loop = loop.BaseType;
      }
    }

    private static void SetPluginInterfaces(PluginDescriptor plugin, Type type)
    {
      foreach (var interfaceType in type.GetInterfaces())
        plugin.Interfaces.Add(new QualifiedName(interfaceType.AssemblyQualifiedName));
    }

    private static void SetPluginSettings(PluginDescriptor plugin, Type type)
    {
      // Fetch plugin settings
      foreach (var property in type.GetProperties())
      {
        CustomAttributeData attribute = CustomAttributeData.GetCustomAttributes(property).FirstOrDefault(x => x.Constructor.DeclaringType.FullName == typeof(PluginSettingAttribute).FullName);
        if (attribute == null)
          continue;

        PluginSettingDescriptor setting = new PluginSettingDescriptor();
        setting.Name = property.Name;
        setting.SettingType = property.PropertyType.AssemblyQualifiedName;

        foreach (var namedArg in attribute.NamedArguments)
        {
          if (namedArg.MemberInfo.Name == "Name")
            setting.Name = (string)namedArg.TypedValue.Value;
          else if (namedArg.MemberInfo.Name == "Required")
            setting.Required = (bool)namedArg.TypedValue.Value;
        }

        plugin.Settings.Add(setting);
      }
    }

    public event EventHandler<PluginEventArgs> PluginAdded;
    public event EventHandler<PluginEventArgs> PluginRemoved;
  }
}
