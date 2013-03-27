using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PluginFramework.Logging;
using PluginFramework.Tests.Mocks;

namespace PluginFramework.Tests.Logging
{
  [TestClass]
  public class UnitTest_ILogger
  {
    [TestMethod]
    public void GetLogWithTypeThrowsOnNullType()
    {
      ILogger tested = new MockILogger();
      DoAssert.Throws<ArgumentNullException>(() => tested.GetLog((Type)null));
    }

    [TestMethod]
    public void GetLogWithTypeReturnsALoggerWithTypeName()
    {
      string expected = "PluginFramework.Tests.Logging.UnitTest_ILogger";
      string actual = null;

      ILogger tested = new MockILogger();
      actual = tested.GetLog(GetType()).Name;

      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void GetLogWithObjectThrowsOnNullObject()
    {
      ILogger tested = new MockILogger();
      DoAssert.Throws<ArgumentNullException>(() => tested.GetLog((object)null));
    }

  }
}
