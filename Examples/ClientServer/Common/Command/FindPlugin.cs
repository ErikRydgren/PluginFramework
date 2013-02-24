using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MassTransit;

namespace PluginFramework.Command
{
  [Serializable]
  public class FindPlugin
  {
    public PluginFilter Filter { get; set; }
  }
}
