using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PluginFramework.Logging;
using System.Diagnostics;
using Moq;

namespace PluginFramework.Tests
{
  [TestClass]
  public class UnitTest_TraceLogger
  {
    [TestMethod]
    public void ReturnsTheNameItWasCreatedWith()
    {
      string expected = "loggername";
      TraceLogger tested = new TraceLogger(expected);
      string actual = tested.Name;
      Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void DefaultTraceIsTraceWrapper()
    {
      TraceLogger tested = new TraceLogger("name");
      Assert.IsInstanceOfType(tested.Trace, typeof(TraceWrapper));
    }

    [TestMethod]
    public void DebugMessageWritesAsTraceInformation()
    {
      string message = "message";
      string logname = "logger";
      var mockTrace = new Mock<ITrace>();
      mockTrace.Setup(x => x.TraceInformation("[{0}] {1} {2}", "DEBUG", logname, message)).Verifiable();
      ITrace trace = mockTrace.Object;
      ILog tested = new TraceLogger(logname, trace);
      tested.Debug(message);
      mockTrace.Verify();
    }

    [TestMethod]
    public void FormattedDebugMessageWritesAsTraceInformation()
    {
      string format = "message {0}";
      string arg = "theArg";
      string logname = "logger";
      var mockTrace = new Mock<ITrace>();
      mockTrace.Setup(x => x.TraceInformation("[{0}] {1} {2}", "DEBUG", logname, string.Format(format, arg))).Verifiable();
      ITrace trace = mockTrace.Object;
      ILog tested = new TraceLogger(logname, trace);
      tested.Debug(format, arg);
      mockTrace.Verify();      
    }

    [TestMethod]
    public void InfoMessageWritesAsTraceInformation()
    {
      string message = "message";
      string logname = "logger";
      var mockTrace = new Mock<ITrace>();
      mockTrace.Setup(x => x.TraceInformation("[{0}] {1} {2}", "INFO", logname, message)).Verifiable();
      ITrace trace = mockTrace.Object;
      ILog tested = new TraceLogger(logname, trace);
      tested.Info(message);
      mockTrace.Verify();
    }

    [TestMethod]
    public void FormattedInfoMessageWritesAsTraceInformation()
    {
      string format = "message {0}";
      string arg = "theArg";
      string logname = "logger";
      var mockTrace = new Mock<ITrace>();
      mockTrace.Setup(x => x.TraceInformation("[{0}] {1} {2}", "INFO", logname, string.Format(format, arg))).Verifiable();
      ITrace trace = mockTrace.Object;
      ILog tested = new TraceLogger(logname, trace);
      tested.Info(format, arg);
      mockTrace.Verify();
    }

    [TestMethod]
    public void WarnMessageWritesAsTraceInformation()
    {
      string message = "message";
      string logname = "logger";
      var mockTrace = new Mock<ITrace>();
      mockTrace.Setup(x => x.TraceInformation("[{0}] {1} {2}", "WARN", logname, message)).Verifiable();
      ITrace trace = mockTrace.Object;
      ILog tested = new TraceLogger(logname, trace);
      tested.Warn(message);
      mockTrace.Verify();
    }

    [TestMethod]
    public void FormattedWarnMessageWritesAsTraceInformation()
    {
      string format = "message {0}";
      string arg = "theArg";
      string logname = "logger";
      var mockTrace = new Mock<ITrace>();
      mockTrace.Setup(x => x.TraceInformation("[{0}] {1} {2}", "WARN", logname, string.Format(format, arg))).Verifiable();
      ITrace trace = mockTrace.Object;
      ILog tested = new TraceLogger(logname, trace);
      tested.Warn(format, arg);
      mockTrace.Verify();
    }

    [TestMethod]
    public void ErrorMessageWritesAsTraceError()
    {
      string message = "message";
      string logname = "logger";
      var mockTrace = new Mock<ITrace>();
      mockTrace.Setup(x => x.TraceError("[{0}] {1} {2}", "ERROR", logname, message)).Verifiable();
      ITrace trace = mockTrace.Object;
      ILog tested = new TraceLogger(logname, trace);
      tested.Error(message);
      mockTrace.Verify();
    }

    [TestMethod]
    public void FormattedErrorMessageWritesAsTraceInformation()
    {
      string format = "message {0}";
      string arg = "theArg";
      string logname = "logger";
      var mockTrace = new Mock<ITrace>();
      mockTrace.Setup(x => x.TraceError("[{0}] {1} {2}", "ERROR", logname, string.Format(format, arg))).Verifiable();
      ITrace trace = mockTrace.Object;
      ILog tested = new TraceLogger(logname, trace);
      tested.Error(format, arg);
      mockTrace.Verify();
    }

    [TestMethod]
    public void FatalMessageWritesAsTraceError()
    {
      string message = "message";
      string logname = "logger";
      var mockTrace = new Mock<ITrace>();
      mockTrace.Setup(x => x.TraceError("[{0}] {1} {2}", "FATAL", logname, message)).Verifiable();
      ITrace trace = mockTrace.Object;
      ILog tested = new TraceLogger(logname, trace);
      tested.Fatal(message);
      mockTrace.Verify();
    }

    [TestMethod]
    public void FormattedFatalMessageWritesAsTraceInformation()
    {
      string format = "message {0}";
      string arg = "theArg";
      string logname = "logger";
      var mockTrace = new Mock<ITrace>();
      mockTrace.Setup(x => x.TraceError("[{0}] {1} {2}", "FATAL", logname, string.Format(format, arg))).Verifiable();
      ITrace trace = mockTrace.Object;
      ILog tested = new TraceLogger(logname, trace);
      tested.Fatal(format, arg);
      mockTrace.Verify();
    }
  }
}
