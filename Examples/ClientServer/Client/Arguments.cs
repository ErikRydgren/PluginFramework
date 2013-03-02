using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PluginFramework
{
  [Description("PluginFramework Client usage:")]
  internal class Arguments
  {
    [Args.ArgsMemberSwitch("h", "help", "?")]
    [Description("Writes this message")]
    public bool Help { get; set; }

    [DefaultValue("Wcf")]
    [Description("The transport to use. Can be Wcf or Masstransit (note! Masstransit requires installed RabbitMQ)")]
    public string Transport { get; set; }

    [DefaultValue(true)]
    [Description("True to enable caching, false otherwise")]
    public bool Caching { get; set; }

    [DefaultValue(10)]
    [Description("Cache TTL in seconds")]
    public int TTL { get; set; }
  }
}
