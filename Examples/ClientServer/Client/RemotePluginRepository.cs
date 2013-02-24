using System;
using System.Collections.Generic;
using Castle.Core.Logging;
using MassTransit;
using PluginFramework.Command;

namespace PluginFramework
{
  class RemotePluginRepository : IPluginRepository
  {
    ILogger log;
    IServiceBus bus;

    public RemotePluginRepository(IServiceBus bus, ILogger log)
    {
      this.log = log;
      this.bus = bus;
    }

    public IEnumerable<PluginDescriptor> Plugins<T>(PluginFilter filter = null)
    {
      PluginFilter combinedFilter = Plugin.Implements<T>() | Plugin.DerivesFrom<T>();
      if (filter != null)
        combinedFilter = combinedFilter & filter;
      return this.Plugins(combinedFilter);
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
  }
}
