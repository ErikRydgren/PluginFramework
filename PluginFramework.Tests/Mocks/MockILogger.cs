using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginFramework.Logging;
using Moq;

namespace PluginFramework.Tests.Mocks
{
  public class MockILogger : ILogger
  {
    public ILoggerFactory LoggerFactory { get; set; }

    public ILog GetLog(string name)
    {
      var mockLog = new Mock<ILog>();
      mockLog.Setup(x => x.Name).Returns(name);
      return mockLog.Object;
    }
  }
}
