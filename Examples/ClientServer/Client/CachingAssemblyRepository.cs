using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;

namespace PluginFramework
{
  internal struct AssemblyCache
  {
    public DateTime TTL;
    public byte[] Data;
  }

  public class CachingAssemblyRepository : MarshalByRefObject, IAssemblyRepository
  {
    IAssemblyRepository repository;
    TimeSpan TTL;
    ILogger log;
    Dictionary<string, AssemblyCache> cache = new Dictionary<string, AssemblyCache>();

    public CachingAssemblyRepository(IAssemblyRepository repository, TimeSpan TTL, ILogger log)
    {
      this.repository = repository;
      this.TTL = TTL;
      this.log = log;
    }

    public byte[] Get(string assemblyFullName)
    {
      AssemblyCache cached;
      lock (this.cache)
      {
        if (this.cache.TryGetValue(assemblyFullName, out cached) && cached.TTL > DateTime.Now)
        {
          log.DebugFormat("Cache hit for {0}", assemblyFullName);
          return cached.Data;
        }
      }

      byte[] data = this.repository.Get(assemblyFullName);
      if (data != null)
      {
        cached = new AssemblyCache();
        cached.TTL = DateTime.Now + this.TTL;
        cached.Data = data;
        lock (this.cache)
        {
          this.cache[assemblyFullName] = cached;
        }
      }
      return data;
    }
  }
}
