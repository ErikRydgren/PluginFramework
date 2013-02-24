using System;
using System.IO;
using System.Linq;
using PluginFramework;
using PluginInterface;
using System.Collections.Generic;

namespace SingleApplication
{
  class Program
  {
    PluginRepository pluginRepository;
    AssemblyContainer assemblyContainer;
    PluginCreator pluginCreator;

    private void Init()
    {
      AppDomain.CurrentDomain.AssemblyLoad += (s, e) => Console.WriteLine("{0} loaded {1}", AppDomain.CurrentDomain.FriendlyName, e.LoadedAssembly.FullName);

      Console.WriteLine("Creating plugin repository");
      this.pluginRepository = new PluginRepository();
      this.assemblyContainer = new AssemblyContainer();
      this.pluginRepository.AddPluginSource(new AssemblySourcePluginSource(this.assemblyContainer));

      DirectoryInfo pluginDir = new DirectoryInfo(@"..\..\..\Plugin\Bin");
      Console.WriteLine(@"Adding plugins from {0}", pluginDir.FullName);
      this.assemblyContainer.SyncWithDirectory(pluginDir, true);

      this.pluginCreator = new PluginCreator(this.assemblyContainer);
    }

    private void QueryAndRunPlugins()
    {
      Console.WriteLine("Creating AppDomain PluginDomain");
      AppDomain pluginDomain = AppDomain.CreateDomain("PluginDomain");

      AppDomain.CurrentDomain.AssemblyLoad += (s, e) => Console.WriteLine("{0} loaded {1}", AppDomain.CurrentDomain.FriendlyName, e.LoadedAssembly.FullName);
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

    private void Run()
    {
      try
      {
        Init();

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

    static void Main(string[] args)
    {
      new Program().Run();
    }
  }
}
