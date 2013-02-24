using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MassTransit;
using PluginFramework.Command;
using Castle.Core.Logging;

namespace PluginFramework.Handlers
{
  public class FindPluginHandler : Consumes<FindPlugin>.Context
  {
    ILogger log;
    IPluginRepository pluginRepository;

    public FindPluginHandler(IPluginRepository pluginRepository, ILogger log)
    {
      this.log = log;
      this.pluginRepository = pluginRepository;
    }

    public void Consume(IConsumeContext<FindPlugin> context)
    {
      FindPlugin message = context.Message;

      if (log.IsDebugEnabled)
        log.DebugFormat("Handling command FindPlugin {0}", message.Filter);

      IEnumerable<PluginDescriptor> plugins = pluginRepository.Plugins(message.Filter);

      FindPluginResponse reply = new FindPluginResponse(message);
      reply.FoundPlugins = plugins.ToArray();
      
      context.Respond(reply, cb => cb.SetRetryCount(0));
    }

  }
}
