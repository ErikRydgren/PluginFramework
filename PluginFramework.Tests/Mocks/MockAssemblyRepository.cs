using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginFramework.Tests.Mocks
{
  public class MockAssemblyRepository : MarshalByRefObject, IAssemblyRepository
  {
    public Dictionary<string, byte[]> Fetched { get; private set; }

    IAssemblyRepository repos;
    public MockAssemblyRepository()
    {
      var container = new AssemblyContainer();
      container.Add(typeof(MockPlugin1).Assembly.Location);
      container.Add(typeof(PluginCreator).Assembly.Location);
      this.repos = container;
      this.Fetched = new Dictionary<string, byte[]>();
    }

    public byte[] Fetch(string assemblyFullName)
    {
      byte[] data = this.repos.Fetch(assemblyFullName);
      this.Fetched.Add(assemblyFullName, data);
      return data;
    }
  }
}
