using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PluginFramework.Logging;

namespace PluginFramework.Tests.Logging
{
  [TestClass]
  public class UnitTest_ILog
  {
    [TestMethod]
    public void DebugWithoutFormatProviderUsesInvariantFormatter()
    {
      var mockLog = new Mock<ILog>();
      mockLog.Setup(x => x.Debug(System.Globalization.CultureInfo.InvariantCulture, It.IsAny<string>(), It.IsAny<object[]>())).Verifiable();
      ILog tested = mockLog.Object;
      tested.Debug("{0}", "test");
      mockLog.Verify();
    }

    [TestMethod]
    public void InfoWithoutFormatProviderUsesInvariantFormatter()
    {
      var mockLog = new Mock<ILog>();
      mockLog.Setup(x => x.Info(System.Globalization.CultureInfo.InvariantCulture, It.IsAny<string>(), It.IsAny<object[]>())).Verifiable();
      ILog tested = mockLog.Object;
      tested.Info("{0}", "test");
      mockLog.Verify();
    }

    [TestMethod]
    public void WarnWithoutFormatProviderUsesInvariantFormatter()
    {
      var mockLog = new Mock<ILog>();
      mockLog.Setup(x => x.Warn(System.Globalization.CultureInfo.InvariantCulture, It.IsAny<string>(), It.IsAny<object[]>())).Verifiable();
      ILog tested = mockLog.Object;
      tested.Warn("{0}", "test");
      mockLog.Verify();
    }

    [TestMethod]
    public void ErrorWithoutFormatProviderUsesInvariantFormatter()
    {
      var mockLog = new Mock<ILog>();
      mockLog.Setup(x => x.Error(System.Globalization.CultureInfo.InvariantCulture, It.IsAny<string>(), It.IsAny<object[]>())).Verifiable();
      ILog tested = mockLog.Object;
      tested.Error("{0}", "test");
      mockLog.Verify();
    }

    [TestMethod]
    public void FatalWithoutFormatProviderUsesInvariantFormatter()
    {
      var mockLog = new Mock<ILog>();
      mockLog.Setup(x => x.Fatal(System.Globalization.CultureInfo.InvariantCulture, It.IsAny<string>(), It.IsAny<object[]>())).Verifiable();
      ILog tested = mockLog.Object;
      tested.Fatal("{0}", "test");
      mockLog.Verify();
    }
  
  }
}
