using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginFramework.Tests
{
  [TestClass]
  public sealed class UnitTest_PluginInfoAttribute
  {
    #region Construction
    [TestMethod]
    public void ConstructionWithKeyAndValue()
    {
      var key = "theKey";
      var value = "theValue";

      PluginInfoAttribute tested = new PluginInfoAttribute(key, value);
      Assert.AreEqual(key, tested.Key);
      Assert.AreEqual(value, tested.Value);
    }
    #endregion
  }
}
