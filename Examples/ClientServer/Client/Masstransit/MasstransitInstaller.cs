using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using MassTransit;

namespace PluginFramework.Installers
{
  public class MasstransitClientInstaller : IWindsorInstaller
  {
    public void Install(Castle.Windsor.IWindsorContainer container, Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore store)
    {
      container.Register(
        Classes.FromThisAssembly().BasedOn<IConsumer>(),

        Component.For<IServiceBus>().LifestyleSingleton().UsingFactoryMethod((kernel, context) =>
          ServiceBusFactory.New(conf =>
          {
            conf.UseRabbitMq();
            conf.ReceiveFrom("rabbitmq://localhost/PluginFramework-Client");
            conf.Subscribe(x => x.LoadFrom(container));
          })),

        Component.For<IPluginRepository, IAssemblyRepository>().Named("Masstransit").ImplementedBy<MasstransitClient>().LifestyleSingleton(),
        Component.For<IAssemblyRepository>().Named("MasstransitCaching").ImplementedBy<CachingAssemblyRepository>().LifestyleSingleton()
          .DependsOn(ServiceOverride.ForKey<IAssemblyRepository>().Eq("Masstransit"))
          .DependsOn(new { TTL = TimeSpan.FromSeconds(10) }),

        Component.For<Client>().Named("MasstransitClient").ImplementedBy<Client>().LifestyleTransient()
          .DependsOn(ServiceOverride.ForKey<IPluginRepository>().Eq("Masstransit"))
          .DependsOn(ServiceOverride.ForKey<IAssemblyRepository>().Eq("MasstransitCaching")) // Change "MasstransitCaching" to "Masstransit" to bypass cache
      );
    }
  }
}
