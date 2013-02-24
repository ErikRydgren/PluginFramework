using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MassTransit;

namespace PluginFramework.Command
{
  [Serializable]
  public class FindPluginResponse
  {
    public FindPluginResponse(FindPlugin request)
    {
      this.Request = request;
    }

    public FindPlugin Request { get; set; }
    public PluginDescriptor[] FoundPlugins { get; set; }
  }
}
