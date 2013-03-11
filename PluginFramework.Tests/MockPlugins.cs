using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginFramework.Tests
{
  internal interface IMockPluginInterface1
  {
  }

  internal interface IMockPluginInterface2
  {
  }

  internal class MockPluginBase : MarshalByRefObject
  {
  }

  [Plugin]
  internal class MockPlugin1 : MockPluginBase, IMockPluginInterface1
  {
    [PluginSetting]
    public int Setting { get; set; }
  }

  [Plugin(Name = "MockPlugin2")]
  internal class MockPlugin2 : MarshalByRefObject, IMockPluginInterface2
  {
    [PluginSetting(Required=true, Name="NamedSetting")]
    public int Setting { get; set; }
  }

  [Plugin(Name = "MockPlugin3")]
  internal class MockPlugin3 : MarshalByRefObject, IMockPluginInterface1
  {
    [PluginSetting(Required = true, Name = "NamedSetting")]
    public int Setting { get; set; }
  }

}