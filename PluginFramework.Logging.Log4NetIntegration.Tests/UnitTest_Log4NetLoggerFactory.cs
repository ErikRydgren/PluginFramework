using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginFramework.Logging.Log4NetIntegration.Tests
{
  [TestClass]
  public class UnitTest_Log4NetLoggerFactory
  {
    [TestMethod]
    public void GetShouldReturnNamedLog4NetLogger()
    {
      var expected = "somename";
      Log4NetLoggerFactory tested = new Log4NetLoggerFactory();
      ILog log = tested.GetLog(expected);
      var actual = log.Name;
      Assert.IsInstanceOfType(log, typeof(ILog));
      Assert.IsInstanceOfType(log, typeof(Log4NetLogger));
      Assert.AreEqual(expected, actual);
    }
  }
}
