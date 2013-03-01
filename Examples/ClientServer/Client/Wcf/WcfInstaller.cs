using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;

namespace PluginFramework.Installers
{
  public class WcfClientInstaller : IWindsorInstaller
  {
    public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
    {
      container.Register(
        Component.For<IPluginRepository, IAssemblyRepository>().Named("Wcf").ImplementedBy<WcfServiceClient>().LifestyleSingleton(),
        Component.For<IAssemblyRepository>().Named("WcfCaching").ImplementedBy<CachingAssemblyRepository>().LifestyleSingleton()
          .DependsOn(ServiceOverride.ForKey<IAssemblyRepository>().Eq("Wcf"))
          .DependsOn(new { TTL = TimeSpan.FromSeconds(10) }),

        Component.For<Client>().Named("WcfClient").ImplementedBy<Client>().LifestyleTransient()
          .DependsOn(ServiceOverride.ForKey<IPluginRepository>().Eq("Wcf"))
          .DependsOn(ServiceOverride.ForKey<IAssemblyRepository>().Eq("WcfCaching")) // Change "WcfCaching" to "Wcf" to bypass cache
      );
    }
  }
}
