using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginFramework;
using PluginInterface;

namespace Plugins
{
  [Plugin]
  [PluginVersion(1, 0)]
  public class TestPluginWithDependancy : Dependancy.PluginBase
  {
    public TestPluginWithDependancy()
    {
      this.MyName = "DefaultName";
    }

    [PluginSetting]
    public string MyName { get; set; }

    public override void SayHello()
    {
      Console.WriteLine("Hello! My name is {0} and I am a {1} running inside domain {2}", this.MyName, GetType().AssemblyQualifiedName, AppDomain.CurrentDomain.FriendlyName);
    }
  }
}
