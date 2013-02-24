using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginInterface;

namespace Dependancy
{
  public abstract class PluginBase : MarshalByRefObject, ITestPlugin
  {
    public abstract void SayHello();
  }

}
