using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginFramework
{
  [Serializable]
  public class PluginDescriptor
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
  }
}
