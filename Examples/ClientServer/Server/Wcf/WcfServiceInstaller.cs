using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;

namespace PluginFramework.Installers
{
  public class WcfServiceInstaller : IWindsorInstaller
  {
    public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
    {
      container.Register(Component.For<IWcfService>().ImplementedBy <WcfService>().LifestyleSingleton());
    }
  }
}
