using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginFramework.Tests.Mocks
{
  class MockPluginDirectory : IPluginDirectory
  {
    public void RaiseFileFound(string filePath)
    {
      if (this.FileFound != null)
        this.FileFound(this, new PluginDirectoryEventArgs(filePath));
    }

    public void RaiseFileLost(string filePath)
    {
      if (this.FileLost != null)
        this.FileLost(this, new PluginDirectoryEventArgs(filePath));
    }

    public string Path { get { return "mockpath"; } }

    public event EventHandler<PluginDirectoryEventArgs> FileFound;
    public event EventHandler<PluginDirectoryEventArgs> FileLost;

    public void Dispose()
    {
    }
  }
}
