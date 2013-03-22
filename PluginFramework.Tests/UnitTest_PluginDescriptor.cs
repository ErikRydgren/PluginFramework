using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginFramework.Tests
{
  [TestClass]
  public class UnitTest_PluginDescriptor
  {
    [TestMethod]
    public void ConstructionDefault()
    {
      PluginDescriptor tested = new PluginDescriptor();
      Assert.IsNotNull(tested.Interfaces);
      Assert.IsNotNull(tested.Derives);
      Assert.IsNotNull(tested.InfoValues);
      Assert.IsNotNull(tested.Settings);
    }

    [TestMethod]
    public void EqualOnlyIfSameQualifiedName()
    {
      PluginDescriptor tested = new PluginDescriptor()
      {
        QualifiedName = typeof(string)
      };

      PluginDescriptor sameName = new PluginDescriptor()
      {
        QualifiedName = typeof(string)
      };

      PluginDescriptor otherName = new PluginDescriptor()
      {
        QualifiedName = typeof(int)
      };

      Assert.IsTrue(tested.Equals(sameName));
      Assert.IsFalse(tested.Equals(otherName));
    }

    [TestMethod]
    public void NotEqualToNull()
    {
      PluginDescriptor tested = new PluginDescriptor()
      {
        QualifiedName = typeof(string)
      };

      Assert.IsFalse(tested.Equals(null));
    }

    [TestMethod]
    public void NotEqualIfComparedToOtherType()
    {
      PluginDescriptor tested = new PluginDescriptor()
      {
        QualifiedName = typeof(string)
      };

      object other = "object";

      Assert.IsFalse(tested.Equals(other));
    }
  }
}
