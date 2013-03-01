using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace PluginFramework
{
  [ServiceContract]
  public interface IWcfService
  {
    [OperationContract]
    IEnumerable<PluginDescriptor> Plugins(PluginFilter filter = null);

    [OperationContract]
    byte[] Get(string assemblyFullName);
  }
}
