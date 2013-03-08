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
  using Castle.Core.Logging;
  using MassTransit;

  class MasstransitClient : MarshalByRefObject, IPluginRepository, IAssemblyRepository
  {
    ILogger log;
    IServiceBus bus;

    public MasstransitClient(IServiceBus bus, ILogger log)
    {
      this.log = log;
      this.bus = bus;
    }

    public IEnumerable<PluginDescriptor> Plugins(PluginFilter filter)
    {
      this.log.DebugFormat("Finding plugins statisfying {0}", filter);

      FindPlugin cmd = new FindPlugin() { Filter = filter };
      PluginDescriptor[] foundPlugins = new PluginDescriptor[0];
      this.bus.PublishRequest(cmd, cb =>
      {
        // cb.SetRequestExpiration(TimeSpan.FromSeconds(10)); <--- Bug that causes exception on RabbitMq (fixed in trunk but not on NuGet)
        cb.HandleTimeout(TimeSpan.FromSeconds(10), msg =>
        {
          this.log.WarnFormat("Timeout requesting {0}", filter);
        });

        cb.Handle<FindPluginResponse>((context, message) =>
        {
          foundPlugins = message.FoundPlugins;
          this.log.DebugFormat("Found {0} plugins for {1}", foundPlugins.Length, filter);
        });
      });
      return foundPlugins;
    }

    public byte[] Fetch(string assemblyFullName)
    {
      byte[] bytes = null;

      log.DebugFormat("Requesting {0}", assemblyFullName);
      FetchAssembly request = new FetchAssembly();
      request.Name = assemblyFullName;
      this.bus.PublishRequest(request, callback =>
      {
        callback.SetTimeout(TimeSpan.FromSeconds(10));

        callback.Handle<FetchAssemblyResponse>((context, message) =>
        {
          bytes = message.Bytes;
          log.DebugFormat("Got {0} bytes for {1}", bytes.Length, assemblyFullName);
        });

        callback.HandleTimeout(TimeSpan.FromSeconds(10), cb =>
        {
          log.DebugFormat("Timeout while fetching {0}", assemblyFullName);
        });
      });

      return bytes;
    }
  }
}
