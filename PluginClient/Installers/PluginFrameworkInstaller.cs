using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;

namespace PluginFramework.Installers
{
  public class PluginFrameworkInstaller : IWindsorInstaller
  {
    public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
    {
      container.Register(
        Component.For<IPluginRepository>().LifestyleSingleton().ImplementedBy<RemotePluginRepository>(),
        Component.For<IAssemblyRepository>().LifestyleSingleton().ImplementedBy<RemoteAssemblyRepository>(),
        Component.For<PluginCreator>().LifestyleSingleton()
      );
    }
  }
}
