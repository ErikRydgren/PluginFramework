using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace PluginFramework.Logging.Log4NetIntegration.Tests
{
  [TestClass]
  public class UnitTest_Log4NetLogger
  {
    [TestMethod]
    public void ConstructorShouldSetInnerLog()
    {
      var expected = new Mock<log4net.ILog>().Object;
      Log4NetLogger tested = new Log4NetLogger(expected);
      var actual = tested.log;
      Assert.IsNotNull(actual);
      Assert.AreSame(expected, actual);
    }

    [TestMethod]
    public void ConstructorRequiresNonNullLog()
    {
      DoAssert.Throws<ArgumentNullException>(() => new Log4NetLogger(null));
    }

    [TestMethod]
    public void DebugShouldForwardsMessageToInnerDebug()
    {
      var message = "somemessage";
      var mockLog = new Mock<log4net.ILog>();
      mockLog.Setup(x => x.Debug(message)).Verifiable();
      ILog tested = new Log4NetLogger(mockLog.Object);
      tested.Debug(message);
      mockLog.Verify();
    }

    [TestMethod]
    public void InfoShouldForwardsMessageToInnerInfo()
    {
      var message = "somemessage";
      var mockLog = new Mock<log4net.ILog>();
      mockLog.Setup(x => x.Info(message)).Verifiable();
      ILog tested = new Log4NetLogger(mockLog.Object);
      tested.Info(message);
      mockLog.Verify();
    }

    [TestMethod]
    public void WarnShouldForwardsMessageToInnerWarn()
    {
      var message = "somemessage";
      var mockLog = new Mock<log4net.ILog>();
      mockLog.Setup(x => x.Warn(message)).Verifiable();
      ILog tested = new Log4NetLogger(mockLog.Object);
      tested.Warn(message);
      mockLog.Verify();
    }

    [TestMethod]
    public void ErrorShouldForwardsMessageToInnerError()
    {
      var message = "somemessage";
      var mockLog = new Mock<log4net.ILog>();
      mockLog.Setup(x => x.Error(message)).Verifiable();
      ILog tested = new Log4NetLogger(mockLog.Object);
      tested.Error(message);
      mockLog.Verify();
    }

    [TestMethod]
    public void FatalShouldForwardsMessageToInnerFatal()
    {
      var message = "somemessage";
      var mockLog = new Mock<log4net.ILog>();
      mockLog.Setup(x => x.Fatal(message)).Verifiable();
      ILog tested = new Log4NetLogger(mockLog.Object);
      tested.Fatal(message);
      mockLog.Verify();
    }

    [TestMethod]
    public void FormattedDebugShouldForwardsMessageToInnerDebugFormat()
    {
      var format = "formatted message";
      var args = new object[] { "arg1", "arg2" };
      var provider = new Mock<IFormatProvider>().Object;
      var mockLog = new Mock<log4net.ILog>();
      mockLog.Setup(x => x.DebugFormat(provider, format, args)).Verifiable();
      ILog tested = new Log4NetLogger(mockLog.Object);
      tested.Debug(provider, format, args);
      mockLog.Verify();
    }

    [TestMethod]
    public void FormattedInfoShouldForwardsMessageToInnerInfoFormat()
    {
      var format = "formatted message";
      var args = new object[] { "arg1", "arg2" };
      var provider = new Mock<IFormatProvider>().Object;
      var mockLog = new Mock<log4net.ILog>();
      mockLog.Setup(x => x.InfoFormat(provider, format, args)).Verifiable();
      ILog tested = new Log4NetLogger(mockLog.Object);
      tested.Info(provider, format, args);
      mockLog.Verify();
    }

    [TestMethod]
    public void FormattedWarnShouldForwardsMessageToInnerWarnFormat()
    {
      var format = "formatted message";
      var args = new object[] { "arg1", "arg2" };
      var provider = new Mock<IFormatProvider>().Object;
      var mockLog = new Mock<log4net.ILog>();
      mockLog.Setup(x => x.WarnFormat(provider, format, args)).Verifiable();
      ILog tested = new Log4NetLogger(mockLog.Object);
      tested.Warn(provider, format, args);
      mockLog.Verify();
    }

    [TestMethod]
    public void FormattedErrorShouldForwardsMessageToInnerErrorFormat()
    {
      var format = "formatted message";
      var args = new object[] { "arg1", "arg2" };
      var provider = new Mock<IFormatProvider>().Object;
      var mockLog = new Mock<log4net.ILog>();
      mockLog.Setup(x => x.ErrorFormat(provider, format, args)).Verifiable();
      ILog tested = new Log4NetLogger(mockLog.Object);
      tested.Error(provider, format, args);
      mockLog.Verify();
    }

    [TestMethod]
    public void FormattedFatalShouldForwardsMessageToInnerFatalFormat()
    {
      var format = "formatted message";
      var args = new object[] { "arg1", "arg2" };
      var provider = new Mock<IFormatProvider>().Object;
      var mockLog = new Mock<log4net.ILog>();
      mockLog.Setup(x => x.FatalFormat(provider, format, args)).Verifiable();
      ILog tested = new Log4NetLogger(mockLog.Object);
      tested.Fatal(provider, format, args);
      mockLog.Verify();
    }
  }
}
