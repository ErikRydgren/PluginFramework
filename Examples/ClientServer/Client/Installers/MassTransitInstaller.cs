using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.MicroKernel.SubSystems.Configuration;
using MassTransit;
using MassTransit.WindsorIntegration;

namespace PluginFramework.Installers
{
  public class MassTransitInstaller : IWindsorInstaller
  {
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      container.Register(
        Classes.FromThisAssembly().BasedOn<IConsumer>(),

        Component.For<IServiceBus>().LifestyleSingleton().UsingFactoryMethod((kernel, context) =>
          ServiceBusFactory.New(conf =>
          {
            conf.UseRabbitMq();
            conf.ReceiveFrom("rabbitmq://localhost/PluginFramework-Client");
            conf.Subscribe(x => x.LoadFrom(container));
          }))
      );
    }
  }
}
