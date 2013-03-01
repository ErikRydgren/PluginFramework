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
        Component.For<IPluginRepository>().ImplementedBy<RemotePluginRepository>().LifestyleSingleton(),

        Component.For<IAssemblyRepository>().ImplementedBy<RemoteAssemblyRepository>().LifestyleSingleton(),

        Component.For<IAssemblyRepository>().Named("Caching").ImplementedBy<CachingAssemblyRepository>().LifestyleSingleton()
          .DependsOn(new { TTL = TimeSpan.FromSeconds(10) }),

        Component.For<Client>().ImplementedBy<Client>().LifestyleTransient()
          .DependsOn(ServiceOverride.ForKey<IAssemblyRepository>().Eq("Caching"))
      );
    }
  }
}
