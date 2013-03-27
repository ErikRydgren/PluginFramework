using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginFramework.Logging.NLogIntegration.Tests
{
  [TestClass]
  public class UnitTest_NLogLoggerFactory
  {
    [TestMethod]
    public void GetShouldReturnNamedNLogLogger()
    {
      var expected = "somename";
      NLogLoggerFactory tested = new NLogLoggerFactory();
      ILog log = tested.GetLog(expected);
      var actual = log.Name;
      Assert.IsInstanceOfType(log, typeof(ILog));
      Assert.IsInstanceOfType(log, typeof(NLog.Logger));
      Assert.AreEqual(expected, actual);
    }
  }
}
