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
using PluginInterface;
using Castle.MicroKernel;

namespace PluginFramework
{
  class Client
  {
    ILogger log;
    IPluginRepository pluginRepository;
    PluginCreator pluginCreator;

    public Client(ILogger log, IPluginRepository pluginRepository, PluginCreator pluginCreator)
    {
      this.log = log;
      this.pluginRepository = pluginRepository;
      this.pluginCreator = pluginCreator;
    }

    public void Run()
    {
      try
      {
        AppDomain.CurrentDomain.AssemblyLoad += (s, e) => Console.WriteLine("{0} loaded {1}", AppDomain.CurrentDomain.FriendlyName, e.LoadedAssembly.FullName);

        while (true)
        {
          Console.WriteLine();
          QueryAndRunPlugins();

          Console.WriteLine();
          Console.WriteLine("<Press space to run plugins again or any other key to end>");
          var key = Console.ReadKey(true);
          if (key.Key != ConsoleKey.Spacebar)
            break;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }    
    }

    private void QueryAndRunPlugins()
    {
      Console.WriteLine("Creating AppDomain PluginDomain");
      AppDomain pluginDomain = AppDomain.CreateDomain("PluginDomain");
      pluginDomain.AssemblyLoad += (s, e) => Console.WriteLine("{0} loaded {1}", AppDomain.CurrentDomain.FriendlyName, e.LoadedAssembly.FullName);

      try
      {
        ITestPlugin plugin;
        Dictionary<string, object> settings = new Dictionary<string, object>();
        settings.Add("Name", "SettingName");

        PluginFilter filter = Plugin.Implements<ITestPlugin>() & Plugin.MinVersion("1.0");
        Console.WriteLine("Querying for plugins satisfying {0}", filter);
        PluginDescriptor[] foundPlugins = this.pluginRepository.Plugins(filter).ToArray();
        Console.WriteLine(string.Format("{0} plugins found", foundPlugins.Length));

        foreach (var pluginDescriptor in foundPlugins)
        {
          Console.WriteLine(string.Format("Creating plugin {0} inside {1}", pluginDescriptor.QualifiedName.TypeFullName, pluginDomain.FriendlyName));
          plugin = this.pluginCreator.Create<ITestPlugin>(pluginDescriptor, pluginDomain, settings);
          Console.WriteLine("Say hello plugin...");
          plugin.SayHello();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }

      Console.WriteLine("Unloading {0}", pluginDomain.FriendlyName);
      AppDomain.Unload(pluginDomain);
    }

    static void Main(string[] args)
    {
      // Tell Masstransit to use NLog
      NLogLogger.Use();

      // Setup Windsor container
      using (WindsorContainer container = new WindsorContainer())
      {
        container.AddFacility<LoggingFacility>(f => f.LogUsing(LoggerImplementation.NLog));
        container.Install(FromAssembly.This());
        container.Register(Component.For<Client>());

        // Create and run client
        Client client = container.Resolve<Client>();
        client.Run();
      }
    }
  }
}