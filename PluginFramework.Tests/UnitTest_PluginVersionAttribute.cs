using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginFramework.Tests
{
  [TestClass]
  public sealed class UnitTest_PluginVersionAttribute
  {

    #region Construction
    [TestMethod]
    public void ConstructionWithVersion()
    {
      PluginVersionAttribute att = new PluginVersionAttribute(55,66);
      Assert.AreEqual(55, att.Major);
      Assert.AreEqual(66, att.Minor);
    }
    #endregion
  }
}
