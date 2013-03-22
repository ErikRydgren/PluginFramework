using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PluginFramework.Tests
{
  public static class FileExtension
  {
    public static FileStream WaitAndOpen(string location, FileMode mode, FileAccess access, FileShare share, TimeSpan maxWait)
    {
      DateTime deadline = DateTime.Now + maxWait;
      while (DateTime.Now < deadline)
      {
        try
        {
          FileStream stream = new FileStream(location, mode, access, share);
          return stream;
        }
        catch (IOException)
        {
        }
      }
      return null;
    }
  }
}
