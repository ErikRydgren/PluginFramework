using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using MassTransit;
using PluginFramework.Command;

namespace PluginFramework
{
  class MasstransitClient : MarshalByRefObject, IPluginRepository, IAssemblyRepository
  {
    ILogger log;
    IServiceBus bus;

    public MasstransitClient(IServiceBus bus, ILogger log)
    {
      this.log = log;
      this.bus = bus;
    }

    public IEnumerable<PluginDescriptor> Plugins(PluginFilter filter)
    {
      this.log.DebugFormat("Finding plugins statisfying {0}", filter);

      FindPlugin cmd = new FindPlugin() { Filter = filter };
      PluginDescriptor[] foundPlugins = new PluginDescriptor[0];
      this.bus.PublishRequest(cmd, cb =>
      {
        // cb.SetRequestExpiration(TimeSpan.FromSeconds(10)); <--- Bug that causes exception on RabbitMq (fixed in trunk but not on NuGet)
        cb.HandleTimeout(TimeSpan.FromSeconds(10), msg =>
        {
          this.log.WarnFormat("Timeout requesting {0}", filter);
        });

        cb.Handle<PluginFramework.Command.FindPluginResponse>((context, message) =>
        {
          foundPlugins = message.FoundPlugins;
          this.log.DebugFormat("Found {0} plugins for {1}", foundPlugins.Length, filter);
        });
      });
      return foundPlugins;
    }

    public byte[] Get(string assemblyFullName)
    {
      byte[] bytes = null;

      log.DebugFormat("Requesting {0}", assemblyFullName);
      FetchAssembly request = new FetchAssembly();
      request.Name = assemblyFullName;
      this.bus.PublishRequest(request, callback =>
      {
        callback.SetTimeout(TimeSpan.FromSeconds(10));

        callback.Handle<FetchAssemblyResponse>((context, message) =>
        {
          bytes = message.Bytes;
          log.DebugFormat("Got {0} bytes for {1}", bytes.Length, assemblyFullName);
        });

        callback.HandleTimeout(TimeSpan.FromSeconds(10), cb =>
        {
          log.DebugFormat("Timeout while fetching {0}", assemblyFullName);
        });
      });

      return bytes;
    }
  }
}
