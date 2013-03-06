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
namespace PluginFramework.Examples.SingleApplication
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using PluginFramework;
  using PluginFramework.Examples.TestPlugin;

  class Program : IDisposable
  {
    PluginRepository pluginRepository;
    AssemblyContainer assemblyContainer;

    private void Init()
    {
      AppDomain.CurrentDomain.AssemblyLoad += (s, e) => Console.WriteLine("{0} loaded {1}", AppDomain.CurrentDomain.FriendlyName, e.LoadedAssembly.FullName);

      Console.WriteLine("Creating plugin repository");
      this.pluginRepository = new PluginRepository();
      this.assemblyContainer = new AssemblyContainer();
      this.pluginRepository.AddPluginSource(new AssemblySourceToPluginSource(this.assemblyContainer));

      DirectoryInfo pluginDir = new DirectoryInfo(@"..\..\..\Plugin\Bin");
      Console.WriteLine(@"Adding plugins from {0}", pluginDir.FullName);
      this.assemblyContainer.SyncWithDirectory(pluginDir, true);
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
        Dictionary<string, object> settings = new Dictionary<string, object>()
        {
          { "Setting", "Value" }
        };
        settings.Add("Name", "SettingName");

        PluginFilter filter = Plugin.Implements<ITestPlugin>() & Plugin.MinVersion("1.0");
        Console.WriteLine("Querying for plugins satisfying {0}", filter);
        PluginDescriptor[] foundPlugins = this.pluginRepository.Plugins(filter).ToArray();
        Console.WriteLine(string.Format("{0} plugins found", foundPlugins.Length));

        foreach (var pluginDescriptor in foundPlugins)
        {
          Console.WriteLine(string.Format("Creating plugin {0} inside {1}", pluginDescriptor.QualifiedName.TypeFullName, pluginDomain.FriendlyName));
          plugin = pluginCreator.Create<ITestPlugin>(pluginDescriptor, this.assemblyContainer, settings);
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

    public void Dispose()
    {
      if (this.assemblyContainer != null)
        this.assemblyContainer.Dispose();
    }

    static void Main(string[] args)
    {
      using (var program = new Program())
        program.Run();
    }
  }
}
