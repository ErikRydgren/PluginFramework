using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using MassTransit;
using MassTransit.NLogIntegration.Logging;
using Castle.Facilities.Logging;
using PluginFramework.Command;
using PluginShared;
using Castle.MicroKernel;

namespace PluginFramework
{
  class Client
  {
    static void Main(string[] args)
    {
      // Tell Masstransit to use NLog
      NLogLogger.Use();

      // Setup Windsor container
      WindsorContainer container = new WindsorContainer();
      container.AddFacility<LoggingFacility>(f => f.LogUsing(LoggerImplementation.NLog));
      container.Install(FromAssembly.This());
      var kernel = container.Kernel;

      // Get logger
      ILogger log = kernel.Resolve<ILogger>();

      // Get plugin repository
      IPluginRepository pluginRepository = kernel.Resolve<IPluginRepository>();

      // Search for plugins implementing ITestPlugin
      PluginDescriptor[] foundPlugins = 
        pluginRepository.Plugins(
          Plugin.Implements<ITestPlugin>())
        .ToArray();

      // Fetch plugin creator
      PluginCreator creator = kernel.Resolve<PluginCreator>(); 

      // Create AppDomain for plugins
      AppDomain pluginDomain = AppDomain.CreateDomain("PluginDomain");
      pluginDomain.AssemblyLoad += (s,e) => Console.WriteLine("{0} loaded {1}", AppDomain.CurrentDomain.FriendlyName, e.LoadedAssembly.FullName); 

      // Setup plugin settings
      Dictionary<string, object> settings = new Dictionary<string, object>()
      {
        {"Name", "SettingsName" }
      };
      
      // Create an instance of all found plugins and make them do their thing.
      foreach (var foundPlugin in foundPlugins)
        creator.Create<ITestPlugin>(foundPlugin, pluginDomain).SayHello();
    }
  }
}