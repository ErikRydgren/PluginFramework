using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Moq;
using PluginFramework.Logging;

namespace PluginFramework.Tests.Logging
{
  [TestClass]
  public class UnitTest_TraceWrapper
  {
    [TestMethod]
    public void TraceInformationPassesThrough()
    {
      var target = new System.IO.StringWriter();
      TextWriterTraceListener listener = new TextWriterTraceListener();
      listener.Writer = target;

      string expected = "some message";

      Trace.Listeners.Add(listener);

      ITrace tested = new TraceWrapper();
      tested.TraceInformation(expected);
      
      Assert.IsTrue(target.ToString().Contains(expected));
    }

    [TestMethod]
    public void TraceErrorPassesThrough()
    {
      var target = new System.IO.StringWriter();
      TextWriterTraceListener listener = new TextWriterTraceListener();
      listener.Writer = target;

      string expected = "some message";

      Trace.Listeners.Add(listener);

      ITrace tested = new TraceWrapper();
      tested.TraceError(expected);

      Assert.IsTrue(target.ToString().Contains(expected));
    }

    [TestMethod]
    public void TraceWarningPassesThrough()
    {
      var target = new System.IO.StringWriter();
      TextWriterTraceListener listener = new TextWriterTraceListener();
      listener.Writer = target;

      string expected = "some message";

      Trace.Listeners.Add(listener);

      ITrace tested = new TraceWrapper();
      tested.TraceWarning(expected);

      Assert.IsTrue(target.ToString().Contains(expected));
    }

  }
}
