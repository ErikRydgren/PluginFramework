using System;
using Castle.Core.Logging;
using Castle.Facilities.Logging;
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
    IServiceBus bus;

    public Server(ILogger log, IServiceBus bus)
    {
      this.log = log;
      this.bus = bus;
    }

    public void Start()
    {
      this.log.Info("PluginServer started");
    }

    public void Stop()
    {
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

  class RemoteAssemblyRepository : IAssemblyRepository
  {
    IServiceBus bus;

    public RemoteAssemblyRepository(IServiceBus bus)
    {
      this.bus = bus;
    }

    public byte[] Get(string assemblyFullName)
    {
      FetchAssembly request = new FetchAssembly();
      request.Name = assemblyFullName;

      byte[] bytes = null;

      this.bus.PublishRequest(request, callback =>
        {
          callback.SetTimeout(TimeSpan.FromSeconds(10));
          callback.Handle<FetchAssemblyResponse>((context, message) =>
            {
              bytes = message.Bytes;
            });
          callback.HandleTimeout(TimeSpan.FromSeconds(10), cb => {});
        });

      return bytes;
    }
  }
}
