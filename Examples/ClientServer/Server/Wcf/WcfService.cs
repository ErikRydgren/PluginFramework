using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Castle.Core.Logging;

namespace PluginFramework
{
  [ServiceBehavior(InstanceContextMode=InstanceContextMode.Single)]
  public class WcfService : IWcfService
  {
    IAssemblyRepository assemblyRepository;
    IPluginRepository pluginRepository;
    ILogger log;

    public WcfService(IAssemblyRepository assemblyRepository, IPluginRepository pluginRepository, ILogger log)
    {
      this.assemblyRepository = assemblyRepository;
      this.pluginRepository = pluginRepository;
      this.log = log;
    }

    public IEnumerable<PluginDescriptor> Plugins(PluginFilter filter = null)
    {
      log.DebugFormat("Processing request find plugins: {0}", filter);
      return this.pluginRepository.Plugins(filter);
    }

    public byte[] Get(string assemblyFullName)
    {
      log.DebugFormat("Processing request fetch assembly: {0}", assemblyFullName);
      return this.assemblyRepository.Get(assemblyFullName);
    }
  }
}
