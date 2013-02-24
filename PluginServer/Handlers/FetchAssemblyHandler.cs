using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MassTransit;
using PluginFramework.Command;
using Castle.Core.Logging;

namespace PluginFramework.Handlers
{
  public class FetchAssemblyHandler : Consumes<FetchAssembly>.Context
  {
    ILogger log;
    IAssemblyRepository assemblyRepository;

    public FetchAssemblyHandler(IAssemblyRepository assemblyRepository, ILogger log)
    {
      this.log = log;
      this.assemblyRepository = assemblyRepository;
    }

    public void Consume(IConsumeContext<FetchAssembly> context)
    {
      FetchAssembly message = context.Message;

      if (log.IsDebugEnabled)
        log.DebugFormat("Handling command FetchAssembly {0}", message.Name);

      byte[] bytes = assemblyRepository.Get(message.Name);

      FetchAssemblyResponse response = new FetchAssemblyResponse(message);
      response.Bytes = bytes;
      
      context.Respond(response);
    }

  }
}
