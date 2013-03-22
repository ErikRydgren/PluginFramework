using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginFramework.Tests
{
  public static class MockPluginDescriptor
  {
    public static PluginDescriptor For<T>()
    {
      return new PluginDescriptor()
      {
        QualifiedName = typeof(T),
      };
    }

    public static PluginDescriptor For(QualifiedName name)
    {
      return new PluginDescriptor()
      {
        QualifiedName = name,
      };
    }  
  }
}
