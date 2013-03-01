using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace PluginFramework
{

  public class WcfServiceClient : MarshalByRefObject, IAssemblyRepository, IPluginRepository
  {
    ILogger log;
    IWcfService service;

    public WcfServiceClient(ILogger log)
    {
      this.log = log;

      service = ChannelFactory<IWcfService>.CreateChannel(new BasicHttpBinding(), new EndpointAddress("http://localhost/PluginFrameworkWcfService"));
    }

    public IEnumerable<PluginDescriptor> Plugins(PluginFilter filter = null)
    {
      for (int i = 0; i < 30; i++)
        try
        {
          this.log.DebugFormat("Finding plugins statisfying {0}", filter);
          var foundPlugins = service.Plugins(filter);
          this.log.DebugFormat("Found {0} plugins for {1}", foundPlugins.Count(), filter);
          return foundPlugins;
        }
        catch (EndpointNotFoundException)
        {
          this.log.Debug("Endpoint not found, retrying in 1 second");
          System.Threading.Thread.Sleep(1000);
        }
      throw new PluginException("Endpoint not found WcfHost");
    }

    public byte[] Get(string assemblyFullName)
    {
      for (int i = 0; i < 30; i++)
        try
        {
          log.DebugFormat("Requesting {0}", assemblyFullName);
          byte[] bytes = service.Get(assemblyFullName);
          log.DebugFormat("Got {0} bytes for {1}", bytes.Length, assemblyFullName);
          return bytes;
        }
        catch (EndpointNotFoundException)
        {
          this.log.Debug("Endpoint not found, retrying in 1 second");
          System.Threading.Thread.Sleep(1000);
        }
      throw new PluginException("Unable to communicate with WcfHost");
    }
  }
}
