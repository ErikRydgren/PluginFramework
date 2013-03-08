// 
// Copyright (c) 2013, Erik Rydgren, et al. All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
//  - Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//  - Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the 
//    documentation and/or other materials provided with the distribution.
//  - Neither the name of PluginFramework nor the names of its contributors may be used to endorse or promote products derived from this 
//    software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL ERIK RYDGREN OR OTHER CONTRIBUTORS 
// BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
//
namespace PluginFramework.Examples.ClientServer
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using Castle.Core.Logging;

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

    public byte[] Fetch(string assemblyFullName)
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

      data = this.repository.Fetch(assemblyFullName);
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
