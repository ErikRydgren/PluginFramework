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
  using System.ServiceModel;
  using Castle.Core.Logging;
  using Castle.Facilities.Logging;
  using Castle.MicroKernel.Registration;
  using Castle.Windsor;
  using Castle.Windsor.Installer;
  using MassTransit;
  using MassTransit.NLogIntegration.Logging;
  using Topshelf;

  class Server
  {
    ILogger log;
    ServiceHost host;

    public Server(ILogger log, IServiceBus bus, IWcfService wcfService)
    {
      this.log = log;
      this.host = new ServiceHost(wcfService);
    }

    public void Start()
    {
      host.Open();

      this.log.Info("PluginServer started");
    }

    public void Stop()
    {
      host.Close();

      this.log.Info("PluginServer stopped");
    }

    static void Main(string[] args)
    {
      NLogLogger.Use();

      WindsorContainer container = new WindsorContainer();
      container.AddFacility<LoggingFacility>(f => f.LogUsing(LoggerImplementation.NLog));
      container.Install(FromAssembly.This());
      container.Register(Component.For<Server>().LifestyleSingleton());

      Host host = HostFactory.New(conf =>
      {
        conf.Service<Server>(s =>
        {
          s.ConstructUsing(name => container.Resolve<Server>());
          s.WhenStarted(program => program.Start());
          s.WhenStopped(program => program.Stop());
        });
        conf.RunAsLocalSystem();

        conf.SetServiceName("PluginFramework-Server");
        conf.SetDescription("Server for the plugin framework");
        conf.SetDisplayName("PluginFramework-Server");
        conf.UseNLog();
      });

      host.Run();
    }
  }
}
