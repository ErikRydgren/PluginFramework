using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PluginFramework.Logging;

namespace PluginFramework.Tests
{
  [TestClass]
  public class UnitTest_DebugLoggerFactory
  {
    TraceLoggerFactory tested;

    [TestInitialize]
    public void Init()
    {
      tested = new TraceLoggerFactory();
    }

    [TestMethod]
    public void CanReturnANamedLogger()
    {
      string expected = "loggername";
      ILog log = tested.GetLog(expected);
      string actual = log.Name;
      Assert.IsInstanceOfType(log, typeof(TraceLogger));
      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void GetRequiresAName()
    {
      DoAssert.Throws<ArgumentNullException>(() => tested.GetLog(null));
    }
  }
}
