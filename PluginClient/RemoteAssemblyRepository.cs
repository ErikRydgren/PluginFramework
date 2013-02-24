using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using MassTransit;
using MassTransit.NLogIntegration.Logging;
using Castle.Facilities.Logging;
using PluginFramework.Command;

namespace PluginFramework
{
  class RemoteAssemblyRepository : MarshalByRefObject, IAssemblyRepository
  {
    IServiceBus bus;
    ILogger log;

    public RemoteAssemblyRepository(IServiceBus bus, ILogger log)
    {
      this.bus = bus;
      this.log = log;
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
