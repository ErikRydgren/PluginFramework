using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginFramework.Tests
{
  [TestClass]
  public sealed class UnitTest_PluginSettingAttribute
  {
    [TestMethod]
    public void DefaultIsNotRequired()
    {
      PluginSettingAttribute tested = new PluginSettingAttribute();
      Assert.IsFalse(tested.Required);
    }
  }
}
