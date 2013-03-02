using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Castle.Core.Logging;

namespace PluginFramework
{
  /// <summary>
  /// Acts as a cache with defined TTL for results returned from wrapped IAssemblyRepository
  /// </summary>
  public class CachingAssemblyRepository : MarshalByRefObject, IAssemblyRepository, IDisposable
  {
    IAssemblyRepository repository;
    TimeSpan TTL;
    ILogger log;
    Dictionary<string, byte[]> cache = new Dictionary<string, byte[]>();
    List<KeyValuePair<DateTime, string>> cacheTTL = new List<KeyValuePair<DateTime, string>>();
    Timer purgeTimer;

    public CachingAssemblyRepository(IAssemblyRepository repository, TimeSpan TTL, ILogger log)
    {
      this.repository = repository;
      this.TTL = TTL;
      this.log = log;

      this.purgeTimer = new System.Threading.Timer(OnPurge, null, 1000, 1000);
    }

    public byte[] Get(string assemblyFullName)
    {
      byte[] data;
      lock (this.cache)
      {
        if (this.cache.TryGetValue(assemblyFullName, out data))
        {
          log.DebugFormat("Cache hit for {0}", assemblyFullName);
          return data;
        }
      }

      data = this.repository.Get(assemblyFullName);
      if (data != null)
      {
        lock (this.cache)
        {
          this.cache[assemblyFullName] = data;
          this.cacheTTL.Add(new KeyValuePair<DateTime, string>(DateTime.Now + this.TTL, assemblyFullName));
        }
      }
      return data;
    }

    private void OnPurge(object state)
    {
      var assemblyFullNames = this.cacheTTL.TakeWhile(pair => pair.Key < DateTime.Now).Select(pair => pair.Value).ToArray();

      if (assemblyFullNames.Length == 0)
        return;

      this.cacheTTL.RemoveRange(0, assemblyFullNames.Length);

      lock (this.cache)
      {
        foreach (var assemblyFullName in assemblyFullNames)
        {
          log.DebugFormat("Purging {0}", assemblyFullName);
          this.cache.Remove(assemblyFullName);
        }
      }
    }

    public void Dispose()
    {
      this.purgeTimer.Dispose();
    }
  }
}
