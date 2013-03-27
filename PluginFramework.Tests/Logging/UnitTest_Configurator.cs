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
  public class UnitTest_Configurator
  {
    [TestMethod]
    public void UseTraceLoggerSetsGlobalLoggerFactoryToTraceLoggerFactory()
    {
      Configurator tested = new Configurator();
      Logger.Singleton.LoggerFactory = new Mock<ILoggerFactory>().Object;
      tested.UseTraceLogger();
      Assert.IsInstanceOfType(Logger.Singleton.LoggerFactory, typeof(TraceLoggerFactory));
    }
  }
}
