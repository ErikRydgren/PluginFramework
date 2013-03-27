using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginFramework.Logging.Log4NetIntegration.Tests
{
  /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  [TestClass]
  public class UnitTest_ConfiguratorExtension
  {
    [TestMethod]
    public void UseLog4NetLoggerSetsLoggerFactoryToLog4NetLoggerFactory()
    {
      Logger.Configure(x => x.UseLog4NetLogger());
      Assert.IsInstanceOfType(Logger.Singleton.LoggerFactory, typeof(Log4NetLoggerFactory));
    }
  }
}
