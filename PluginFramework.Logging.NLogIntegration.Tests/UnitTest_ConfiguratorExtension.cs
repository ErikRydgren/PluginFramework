using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginFramework.Logging.NLogIntegration.Tests
{
  /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  [TestClass]
  public class UnitTest_ConfiguratorExtension
  {
    [TestMethod]
    public void UseNLogLoggerSetsLoggerFactoryToNLogLoggerFactory()
    {
      Logger.Configure(x => x.UseNLogLogger());
      Assert.IsInstanceOfType(Logger.Singleton.LoggerFactory, typeof(NLogLoggerFactory));
    }
  }
}
