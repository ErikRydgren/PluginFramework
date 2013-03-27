using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PluginFramework;
using PluginFramework.Logging;
using Moq;

namespace PluginFramework.Tests
{
  [TestClass]
  public class UnitTest_Logger
  {
    [TestMethod]
    public void HasSingletonLogger()
    {
      Assert.IsNotNull(Logger.Singleton);
    }

    [TestMethod]
    public void SetLoggerFactoryRequiresNonNullArgument()
    {
      Logger tested = new Logger();
      DoAssert.Throws<ArgumentNullException>(() => tested.LoggerFactory = null);
    }

    [TestMethod]
    public void CanSetLoggerFactory()
    {
      Logger tested = new Logger();
      var mockFactory = new Mock<ILoggerFactory>();
      var factory = mockFactory.Object;
      tested.LoggerFactory = factory;
      Assert.AreSame(factory, tested.LoggerFactory);
    }

    [TestMethod]
    public void UseTraceLoggerSetsLoggerFactoryToTraceLoggerFactory()
    {
      Mock<ILoggerFactory> mockFactory = new Mock<ILoggerFactory>();
      Logger.Configure(x => x.UseTraceLogger());
      Assert.IsInstanceOfType(Logger.Singleton.LoggerFactory, typeof(TraceLoggerFactory));
    }

    [TestMethod]
    public void GetLogCallsGetLogOnCurrentFactoryWithSameName()
    {
      var name = "somename";
      var mockFactory = new Mock<ILoggerFactory>();
      mockFactory.Setup(factory => factory.GetLog(name)).Verifiable();
      Logger tested = new Logger();
      tested.LoggerFactory = mockFactory.Object;
      tested.GetLog(name);
      mockFactory.Verify();
    }
  }
}
