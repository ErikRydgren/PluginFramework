using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PluginFramework.Tests.Mocks
{
  public class MockDomain : IDisposable
  {
    public AppDomain Domain { get; private set; }

    public MockDomain()
    {
      AppDomainSetup domainSetup = new AppDomainSetup();

      DirectoryInfo orgdir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
      string id = Guid.NewGuid().ToString();
      DirectoryInfo dir = orgdir.CreateSubdirectory(id);

      File.Copy(Path.Combine(orgdir.FullName, "PluginFramework.dll"), Path.Combine(dir.FullName, "PluginFramework.dll"));

      domainSetup.ApplicationBase = dir.FullName;
      this.Domain = AppDomain.CreateDomain(id, null, domainSetup);
    }

    public static implicit operator AppDomain(MockDomain mockdomain)
    {
      return mockdomain.Domain;
    }

    public void Dispose()
    {
      var dir = Domain.BaseDirectory;
      AppDomain.Unload(Domain);
      Directory.Delete(dir, true);
      Domain = null;
    }
  }
}
