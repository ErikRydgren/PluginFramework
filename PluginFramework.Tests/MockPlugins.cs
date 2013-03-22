using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginFramework.Tests
{
  public interface IMockPluginInterface1
  {
  }

  public interface IMockPluginInterface2
  {
  }

  public class MockPluginBase : MarshalByRefObject
  {
  }

  [Plugin]
  [PluginInfo("Info1", "Info 1 value")]
  [PluginInfo("Info2", "Info 2 value")]
  [PluginVersion(55, 66)]
  public class MockPlugin1 : MockPluginBase, IMockPluginInterface1, IDisposable
  {
    [PluginSetting]
    public int Setting { get; set; }

    [PluginSetting(Name = "AnotherSetting")]
    public string Setting2 { get; set; }

    public int NonSetting { get; set; }

    public void Dispose()
    {      
    }
  }

  [Plugin(Name = "MockPlugin2")]
  public class MockPlugin2 : MarshalByRefObject, IMockPluginInterface2
  {
    [PluginSetting(Required=true, Name="NamedSetting")]
    public int Setting { get; set; }

    public int NonSetting { get; set; }
  }

  [Plugin(Name = "MockPlugin3")]
  public class MockPlugin3 : MarshalByRefObject, IMockPluginInterface1
  {
    [PluginSetting(Required = true, Name = "NamedSetting")]
    public int Setting { get; set; }

    public int NonSetting { get; set; }
  }

}