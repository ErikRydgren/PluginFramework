using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using MassTransit;
using MassTransit.NLogIntegration.Logging;

namespace PluginFramework.Installers
{
  public class MasstransitClientInstaller : IWindsorInstaller
  {
    public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
    {
      NLogLogger.Use();

      container.Register(
        Classes.FromThisAssembly().BasedOn<IConsumer>(),

        Component.For<IServiceBus>().LifestyleSingleton().UsingFactoryMethod((kernel, context) =>
          ServiceBusFactory.New(conf =>
          {
            conf.UseRabbitMq();
            conf.ReceiveFrom("rabbitmq://localhost/PluginFramework-Client");
            conf.Subscribe(x => x.LoadFrom(container));
          })),

        Component.For<IPluginRepository, IAssemblyRepository>().ImplementedBy<MasstransitClient>().LifestyleSingleton()
      );
    }
  }
}
