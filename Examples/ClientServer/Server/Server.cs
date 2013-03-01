using System;
using System.ServiceModel;
using Castle.Core.Logging;
using Castle.Facilities.Logging;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using MassTransit;
using MassTransit.NLogIntegration.Logging;
using PluginFramework.Command;
using Topshelf;

namespace PluginFramework
{
  class Server
  {
    ILogger log;
    ServiceHost host;

    public Server(ILogger log, IServiceBus bus, IWcfService wcfService)
    {
      this.log = log;
      this.host = new ServiceHost(wcfService);
    }

    public void Start()
    {
      host.Open();

      this.log.Info("PluginServer started");
    }

    public void Stop()
    {
      host.Close();

      this.log.Info("PluginServer stopped");
    }

    static void Main(string[] args)
    {
      NLogLogger.Use();

      WindsorContainer container = new WindsorContainer();
      container.AddFacility<LoggingFacility>(f => f.LogUsing(LoggerImplementation.NLog));
      container.Install(FromAssembly.This());
      container.Register(Component.For<Server>().LifestyleSingleton());

      Host host = HostFactory.New(conf =>
      {
        conf.Service<Server>(s =>
        {
          s.ConstructUsing(name => container.Resolve<Server>());
          s.WhenStarted(program => program.Start());
          s.WhenStopped(program => program.Stop());
        });
        conf.RunAsLocalSystem();

        conf.SetServiceName("PluginFramework-Server");
        conf.SetDescription("Server for the plugin framework");
        conf.SetDisplayName("PluginFramework-Server");
        conf.UseNLog();
      });

      host.Run();
    }
  }
}
