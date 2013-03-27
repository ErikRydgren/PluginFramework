using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PluginFramework.Logging;
using Moq;
using PluginFramework.Tests.Mocks;
using System.Globalization;

namespace PluginFramework.Tests.Logging
{
  [TestClass]
  public class UnitTest_ProxyLog
  {
    [TestMethod]
    public void InheritsMarshalByRefObject()
    {
      ProxyLog tested = new ProxyLog(new Mock<ILog>().Object);
      Assert.IsTrue(tested is MarshalByRefObject);
    }

    [TestMethod]
    public void RequiresInnerILog()
    {
      DoAssert.Throws<ArgumentNullException>(() => new ProxyLog(null));
    }

    [TestMethod]
    public void ExposesInnerLogName()
    {
      var expected = "SomeName";
      var inner = new Mock<ILog>();
      inner.Setup(x => x.Name).Returns(expected);
      ProxyLog tested = new ProxyLog(inner.Object);
      var actual = tested.Name;
      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void PassesThroughDebug()
    {
      string message = "a message";
      MockLog log = new MockLog();
      ProxyLog tested = new ProxyLog(log);
      tested.Debug(message);
      Assert.IsTrue(log.Any(x => x.Level == MockLog.Level.Debug && x.Message == message));
    }

    [TestMethod]
    public void PassesThroughInfo()
    {
      string message = "a message";
      MockLog log = new MockLog();
      ProxyLog tested = new ProxyLog(log);
      tested.Info(message);
      Assert.IsTrue(log.Any(x => x.Level == MockLog.Level.Info && x.Message == message));
    }

    [TestMethod]
    public void PassesThroughWarn()
    {
      string message = "a message";
      MockLog log = new MockLog();
      ProxyLog tested = new ProxyLog(log);
      tested.Warn(message);
      Assert.IsTrue(log.Any(x => x.Level == MockLog.Level.Warn && x.Message == message));
    }

    [TestMethod]
    public void PassesThroughError()
    {
      string message = "a message";
      MockLog log = new MockLog();
      ProxyLog tested = new ProxyLog(log);
      tested.Error(message);
      Assert.IsTrue(log.Any(x => x.Level == MockLog.Level.Error && x.Message == message));
    }

    [TestMethod]
    public void PassesThroughFatal()
    {
      string message = "a message";
      MockLog log = new MockLog();
      ProxyLog tested = new ProxyLog(log);
      tested.Fatal(message);
      Assert.IsTrue(log.Any(x => x.Level == MockLog.Level.Fatal && x.Message == message));
    }

    [TestMethod]
    public void PassesThroughFormattedDebug()
    {
      string format = "message {0}";
      string arg = "arg";
      var formatProvider = CultureInfo.InvariantCulture;
      MockLog log = new MockLog();
      ProxyLog tested = new ProxyLog(log);
      tested.Debug(formatProvider, format, arg);
      Assert.IsTrue(log.Any(x => x.Level == MockLog.Level.Debug && x.Message == string.Format(formatProvider, format, arg)));
    }

    [TestMethod]
    public void PassesThroughFormattedInfo()
    {
      string format = "message {0}";
      string arg = "arg";
      var formatProvider = CultureInfo.InvariantCulture;
      MockLog log = new MockLog();
      ProxyLog tested = new ProxyLog(log);
      tested.Info(formatProvider, format, arg);
      Assert.IsTrue(log.Any(x => x.Level == MockLog.Level.Info && x.Message == string.Format(formatProvider, format, arg)));
    }

    [TestMethod]
    public void PassesThroughFormattedWarn()
    {
      string format = "message {0}";
      string arg = "arg";
      var formatProvider = CultureInfo.InvariantCulture;
      MockLog log = new MockLog();
      ProxyLog tested = new ProxyLog(log);
      tested.Warn(formatProvider, format, arg);
      Assert.IsTrue(log.Any(x => x.Level == MockLog.Level.Warn && x.Message == string.Format(formatProvider, format, arg)));
    }

    [TestMethod]
    public void PassesThroughFormattedError()
    {
      string format = "message {0}";
      string arg = "arg";
      var formatProvider = CultureInfo.InvariantCulture;
      MockLog log = new MockLog();
      ProxyLog tested = new ProxyLog(log);
      tested.Error(formatProvider, format, arg);
      Assert.IsTrue(log.Any(x => x.Level == MockLog.Level.Error && x.Message == string.Format(formatProvider, format, arg)));
    }

    [TestMethod]
    public void PassesThroughFormattedFatal()
    {
      string format = "message {0}";
      string arg = "arg";
      var formatProvider = CultureInfo.InvariantCulture;
      MockLog log = new MockLog();
      ProxyLog tested = new ProxyLog(log);
      tested.Fatal(formatProvider, format, arg);
      Assert.IsTrue(log.Any(x => x.Level == MockLog.Level.Fatal && x.Message == string.Format(formatProvider, format, arg)));
    }
  }
}
