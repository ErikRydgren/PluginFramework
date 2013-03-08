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
  using System.ServiceModel;
  using System.ServiceModel.Channels;
  using Castle.Core.Logging;

  public class WcfServiceClient : MarshalByRefObject, IAssemblyRepository, IPluginRepository
  {
    ILogger log;
    IWcfService service = null;

    public WcfServiceClient(ILogger log)
    {
      this.log = log;

    }

    private IWcfService Service
    {
      get
      {
        if (service == null || (service as IChannel).State == CommunicationState.Faulted)
          service = ChannelFactory<IWcfService>.CreateChannel(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:54321/PluginFrameworkWcfService"));
        return service;
      }
    }

    public IEnumerable<PluginDescriptor> Plugins(PluginFilter filter = null)
    {
      for (int i = 0; i < 30; i++)
        try
        {
          this.log.DebugFormat("Finding plugins statisfying {0}", filter);
          var foundPlugins = this.Service.Plugins(filter);
          this.log.DebugFormat("Found {0} plugins for {1}", foundPlugins.Count(), filter);
          return foundPlugins;
        }
        catch (EndpointNotFoundException)
        {
          this.log.Debug("Endpoint not found, retrying in 1 second");
          System.Threading.Thread.Sleep(1000);
        }
      throw new PluginException("Endpoint not found WcfHost");
    }

    public byte[] Fetch(string assemblyFullName)
    {
      for (int i = 0; i < 30; i++)
        try
        {
          log.DebugFormat("Requesting {0}", assemblyFullName);
          byte[] bytes = this.Service.Get(assemblyFullName);
          log.DebugFormat("Got {0} bytes for {1}", bytes.Length, assemblyFullName);
          return bytes;
        }
        catch (EndpointNotFoundException)
        {
          this.log.Debug("Endpoint not found, retrying in 1 second");
          System.Threading.Thread.Sleep(1000);
        }
      throw new PluginException("Unable to communicate with WcfHost");
    }
  }
}
