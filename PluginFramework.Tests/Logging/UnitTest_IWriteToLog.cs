using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PluginFramework.Logging;
using Moq;
using PluginFramework.Tests.Mocks;

namespace PluginFramework.Tests.Logging
{
  [TestClass]
  public class UnitTest_IWriteToLog
  {
    [TestMethod]
    public void InitSetsALogForThis()
    {
      ILogWriter tested = new MockILogWriter();
      string expected = tested.GetType().FullName;
      tested.InitLog();
      var actual = tested.Log.Name;
      Assert.AreEqual(expected, actual);
    }
  }
}
