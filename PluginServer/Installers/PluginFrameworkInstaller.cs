using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;

namespace PluginFramework.Installers
{
  public class PluginFrameworkInstaller : IWindsorInstaller
  {
    public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
    {
      AssemblyContainer assemblyContainer = new AssemblyContainer();

      PluginRepository pluginRepository = new PluginRepository();
      pluginRepository.AddPluginSource(new AssemblySourcePluginSource(assemblyContainer));

      System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
      DirectoryInfo pluginDir = new DirectoryInfo(settingsReader.GetValue("PluginPath", typeof(string)) as string);
      assemblyContainer.SyncWithDirectory(pluginDir, true);

      container.Register(Component.For<IPluginRepository>().LifestyleSingleton().Instance(pluginRepository));
      container.Register(Component.For<IAssemblyRepository>().LifestyleSingleton().Instance(assemblyContainer));      
    }
  }
}
