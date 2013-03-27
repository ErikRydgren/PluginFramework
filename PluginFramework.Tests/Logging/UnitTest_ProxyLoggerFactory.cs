using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PluginFramework.Logging;
using Moq;

namespace PluginFramework.Tests.Logging
{
  [TestClass]
  public class UnitTest_ProxyLoggerFactory
  {
    [TestMethod]
    public void RequiresInnnerLoggerFactory()
    {
      DoAssert.Throws<ArgumentNullException>(() => new ProxyLoggerFactory(null));
    }

    [TestMethod]
    public void UsesInnerFactoryToCreateLogs()
    {
      var name = "a name";
      var mockLog = new Mock<ILog>();
      mockLog.Setup(x => x.Name).Returns(name);

      var mockFactory = new Mock<ILoggerFactory>();
      mockFactory.Setup(x => x.GetLog(name)).Returns(mockLog.Object).Verifiable();

      ProxyLoggerFactory tested = new ProxyLoggerFactory(mockFactory.Object);
      ILog log = tested.GetLog(name);

      mockFactory.Verify();
    }

    [TestMethod]
    public void WrapsCreatedLogsInsideProxyLogIfLogIsNotMarshalByRefObject()
    {
      var name = "a name";
      var mockLog = new Mock<ILog>();
      mockLog.Setup(x => x.Name).Returns(name);

      var mockFactory = new Mock<ILoggerFactory>();
      mockFactory.Setup(x => x.GetLog(name)).Returns(mockLog.Object);

      ProxyLoggerFactory tested = new ProxyLoggerFactory(mockFactory.Object);
      ILog log = tested.GetLog(name);

      Assert.IsInstanceOfType(log, typeof(ProxyLog));
    }
  }
}
