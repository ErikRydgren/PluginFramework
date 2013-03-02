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
        Component.For<IPluginRepository, IAssemblyRepository>().ImplementedBy<WcfServiceClient>().LifestyleSingleton()
      );
    }
  }
}
