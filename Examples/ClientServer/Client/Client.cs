using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using Castle.Facilities.Logging;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using MassTransit;
using MassTransit.NLogIntegration.Logging;
using PluginFramework.Command;
using PluginInterface;
using PluginFramework.Installers;

namespace PluginFramework
{
  class Client
  {
    ILogger log;
    IPluginRepository pluginRepository;
    IAssemblyRepository assemblyRepository;

    public Client(ILogger log, IPluginRepository pluginRepository, IAssemblyRepository assemblyRepository)
    {
      this.log = log;
      this.pluginRepository = pluginRepository;
      this.assemblyRepository = assemblyRepository;
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
      IPluginCreator pluginCreator = PluginCreator.GetCreator(pluginDomain);

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
          plugin = pluginCreator.Create<ITestPlugin>(pluginDescriptor, this.assemblyRepository, settings);
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

    static void Main(string[] cmdline)
    {
      // Parse commandline into Arguments
      var argsModelDef = Args.Configuration.Configure<Arguments>();
      Arguments args = new Arguments();
      try
      {
        Args.Configuration.Configure<Arguments>().BindModel(args, cmdline);
      }
      catch (Exception)
      {
        args.Help = true;
      }

      // Write help message
      if (args.Help == true)
      {
        var modelHelp = new Args.Help.HelpProvider().GenerateModelHelp(argsModelDef);
        new Args.Help.Formatters.ConsoleHelpFormatter().WriteHelp(modelHelp, Console.Out);
        return;
      }

      // Setup and run client
      using (WindsorContainer container = new WindsorContainer())
      {
        container.AddFacility<LoggingFacility>(f => f.LogUsing(LoggerImplementation.NLog));

        switch (args.Transport.ToLowerInvariant())
        {
          case "wcf":
            container.Install(new WcfClientInstaller());
            break;

          case "masstransit":
            container.Install(new MasstransitClientInstaller());
            break;

          default:
            Console.WriteLine("Supported transports are Wcf and Masstransit");
            return;
        }

        container.Register(
          Component.For<IAssemblyRepository>().Named("Caching").ImplementedBy<CachingAssemblyRepository>().LifestyleSingleton()
            .DependsOn(new { TTL = TimeSpan.FromSeconds(args.TTL) }),

          Component.For<Client>().ImplementedBy<Client>().LifestyleTransient()
          .DependsOn(args.Caching ? ServiceOverride.ForKey<IAssemblyRepository>().Eq("Caching") : null)
        );

        Client client = container.Resolve<Client>();
        client.Run();
      }
    }
  }
}