using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginFramework.Logging;

namespace PluginFramework.Tests.Mocks
{
  public class MockILogWriter : ILogWriter
  {
    public ILog Log
    {
      get;
      set;
    }
  }
}
