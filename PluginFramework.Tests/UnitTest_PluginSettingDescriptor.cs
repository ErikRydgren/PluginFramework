using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginFramework.Tests
{
  [TestClass]
  public sealed class UnitTest_PluginSettingDescriptor
  {
    [TestMethod]
    public void ToStringExpectedResult()
    {
      PluginSettingDescriptor tested = new PluginSettingDescriptor() { Name = "testName", Required = true, SettingType = typeof(string).AssemblyQualifiedName };
      string expected = "testName [required] " + typeof(string).AssemblyQualifiedName;
      StringAssert.Equals(expected, tested.ToString());

      tested.Required = false;

      expected = "testName [required] " + typeof(string).AssemblyQualifiedName;
      StringAssert.Equals(expected, tested.ToString());
    }
  }
}
