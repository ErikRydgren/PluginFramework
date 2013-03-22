using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginFramework.Tests
{
  [TestClass]
  public class UnitTest_AssemblySourceToPluginSource
  {
    #region Construction
    [TestMethod]
    public void ConstructionRequiresIAssemblySource()
    {
      DoAssert.Throws<ArgumentNullException>(() => new AssemblySourceToPluginSource(null));
    }

    [TestMethod]
    public void ConstructionWithIAssemblySource()
    {
      MockAssemblySource mockSource = new MockAssemblySource();
      AssemblySourceToPluginSource tested = new AssemblySourceToPluginSource(mockSource);
      Assert.IsNotNull(tested);
    }
    #endregion

    #region OnAssemblyAdded
    [TestMethod]
    public void OnAssemblyAddedShouldRaisePluginFoundForEachPluginInAssembly()
    {
      MockAssemblySource mockSource = new MockAssemblySource();
      AssemblySourceToPluginSource tested = new AssemblySourceToPluginSource(mockSource);

      List<PluginDescriptor> plugins = new List<PluginDescriptor>();
      tested.PluginAdded += (s, e) => plugins.Add(e.Plugin);

      using (AssemblyReflectionManager manager = new AssemblyReflectionManager())
      {
        string assemblyPath = GetType().Assembly.Location;
        manager.LoadAssembly(assemblyPath, Guid.NewGuid().ToString());
        AssemblyAddedEventArgs args = new AssemblyAddedEventArgs(assemblyPath, manager);
        mockSource.RaiseAssemblyAdded(args);
      }

      Assert.AreEqual(3, plugins.Count);
      Assert.IsTrue(plugins.Contains(MockPluginDescriptor.For<MockPlugin1>()));
      Assert.IsTrue(plugins.Contains(MockPluginDescriptor.For<MockPlugin2>()));
      Assert.IsTrue(plugins.Contains(MockPluginDescriptor.For<MockPlugin3>()));
    }
    #endregion

    #region OnAssemblyRemoved
    [TestMethod]
    public void OnAssemblyRemovedShouldRaisePluginLostForEachPluginInAssembly()
    {
      MockAssemblySource mockSource = new MockAssemblySource();
      AssemblySourceToPluginSource tested = new AssemblySourceToPluginSource(mockSource);

      List<PluginDescriptor> plugins = new List<PluginDescriptor>();
      tested.PluginAdded += (s, e) => plugins.Add(e.Plugin);
      tested.PluginRemoved += (s, e) => plugins.Remove(e.Plugin);

      using (AssemblyReflectionManager manager = new AssemblyReflectionManager())
      {
        string assemblyPath = GetType().Assembly.Location;
        manager.LoadAssembly(assemblyPath, Guid.NewGuid().ToString());
        mockSource.RaiseAssemblyAdded(new AssemblyAddedEventArgs(assemblyPath, manager));

        AssemblyRemovedEventArgs args = new AssemblyRemovedEventArgs(assemblyPath);
        mockSource.RaiseAssemblyRemoved(args);
      }

      Assert.AreEqual(0, plugins.Count);
    }
    #endregion

    #region Plugin descriptor
    [TestMethod]
    public void PluginDescriptorShouldContainDefaultName()
    {
      MockAssemblySource mockSource = new MockAssemblySource();
      AssemblySourceToPluginSource tested = new AssemblySourceToPluginSource(mockSource);

      List<PluginDescriptor> plugins = new List<PluginDescriptor>();
      tested.PluginAdded += (s, e) => plugins.Add(e.Plugin);
      tested.PluginRemoved += (s, e) => plugins.Remove(e.Plugin);

      using (AssemblyReflectionManager manager = new AssemblyReflectionManager())
      {
        string assemblyPath = GetType().Assembly.Location;
        manager.LoadAssembly(assemblyPath, Guid.NewGuid().ToString());
        mockSource.RaiseAssemblyAdded(new AssemblyAddedEventArgs(assemblyPath, manager));
      }

      PluginDescriptor plugin = plugins.First(x => x.QualifiedName == typeof(MockPlugin1));
      
      Assert.AreEqual("PluginFramework.Tests.MockPlugin1", plugin.Name);
    }

    [TestMethod]
    public void PluginDescriptorShouldContainSpecifiedName()
    {
      MockAssemblySource mockSource = new MockAssemblySource();
      AssemblySourceToPluginSource tested = new AssemblySourceToPluginSource(mockSource);

      List<PluginDescriptor> plugins = new List<PluginDescriptor>();
      tested.PluginAdded += (s, e) => plugins.Add(e.Plugin);
      tested.PluginRemoved += (s, e) => plugins.Remove(e.Plugin);

      using (AssemblyReflectionManager manager = new AssemblyReflectionManager())
      {
        string assemblyPath = GetType().Assembly.Location;
        manager.LoadAssembly(assemblyPath, Guid.NewGuid().ToString());
        mockSource.RaiseAssemblyAdded(new AssemblyAddedEventArgs(assemblyPath, manager));
      }

      PluginDescriptor plugin = plugins.First(x => x.QualifiedName == typeof(MockPlugin2));

      Assert.AreEqual("MockPlugin2", plugin.Name);
    }

    [TestMethod]
    public void PluginDescriptorShouldContainDerivedClasses()
    {
      MockAssemblySource mockSource = new MockAssemblySource();
      AssemblySourceToPluginSource tested = new AssemblySourceToPluginSource(mockSource);

      List<PluginDescriptor> plugins = new List<PluginDescriptor>();
      tested.PluginAdded += (s, e) => plugins.Add(e.Plugin);
      tested.PluginRemoved += (s, e) => plugins.Remove(e.Plugin);

      using (AssemblyReflectionManager manager = new AssemblyReflectionManager())
      {
        string assemblyPath = GetType().Assembly.Location;
        manager.LoadAssembly(assemblyPath, Guid.NewGuid().ToString());
        mockSource.RaiseAssemblyAdded(new AssemblyAddedEventArgs(assemblyPath, manager));
      }

      PluginDescriptor plugin = plugins.First(x => x.QualifiedName == typeof(MockPlugin1));
      Assert.AreEqual(3, plugin.Derives.Count);
      Assert.IsTrue(plugin.Derives.Any(x => x == typeof(MockPluginBase)));
      Assert.IsTrue(plugin.Derives.Any(x => x == typeof(MarshalByRefObject)));
      Assert.IsTrue(plugin.Derives.Any(x => x == typeof(object)));
    }

    [TestMethod]
    public void PluginDescriptorShouldContainImplementedInterfaces()
    {
      MockAssemblySource mockSource = new MockAssemblySource();
      AssemblySourceToPluginSource tested = new AssemblySourceToPluginSource(mockSource);

      List<PluginDescriptor> plugins = new List<PluginDescriptor>();
      tested.PluginAdded += (s, e) => plugins.Add(e.Plugin);
      tested.PluginRemoved += (s, e) => plugins.Remove(e.Plugin);

      using (AssemblyReflectionManager manager = new AssemblyReflectionManager())
      {
        string assemblyPath = GetType().Assembly.Location;
        manager.LoadAssembly(assemblyPath, Guid.NewGuid().ToString());
        mockSource.RaiseAssemblyAdded(new AssemblyAddedEventArgs(assemblyPath, manager));
      }

      PluginDescriptor plugin = plugins.First(x => x.QualifiedName == typeof(MockPlugin1));
      Assert.AreEqual(2, plugin.Interfaces.Count);
      Assert.IsTrue(plugin.Interfaces.Any(x => x == typeof(IMockPluginInterface1)));
      Assert.IsTrue(plugin.Interfaces.Any(x => x == typeof(IDisposable)));
    }

    [TestMethod]
    public void PluginDescriptorShouldContainPluginInfo()
    {
      MockAssemblySource mockSource = new MockAssemblySource();
      AssemblySourceToPluginSource tested = new AssemblySourceToPluginSource(mockSource);

      List<PluginDescriptor> plugins = new List<PluginDescriptor>();
      tested.PluginAdded += (s, e) => plugins.Add(e.Plugin);
      tested.PluginRemoved += (s, e) => plugins.Remove(e.Plugin);

      using (AssemblyReflectionManager manager = new AssemblyReflectionManager())
      {
        string assemblyPath = GetType().Assembly.Location;
        manager.LoadAssembly(assemblyPath, Guid.NewGuid().ToString());
        mockSource.RaiseAssemblyAdded(new AssemblyAddedEventArgs(assemblyPath, manager));
      }

      PluginDescriptor plugin = plugins.First(x => x.QualifiedName == typeof(MockPlugin1));

      Assert.AreEqual(2, plugin.InfoValues.Count);
      Assert.IsTrue(plugin.InfoValues.Any(x => x.Key == "Info1" && x.Value == "Info 1 value"));
      Assert.IsTrue(plugin.InfoValues.Any(x => x.Key == "Info2" && x.Value == "Info 2 value"));
    }

    [TestMethod]
    public void PluginDescriptorShouldContainPluginSettings()
    {
      MockAssemblySource mockSource = new MockAssemblySource();
      AssemblySourceToPluginSource tested = new AssemblySourceToPluginSource(mockSource);

      List<PluginDescriptor> plugins = new List<PluginDescriptor>();
      tested.PluginAdded += (s, e) => plugins.Add(e.Plugin);
      tested.PluginRemoved += (s, e) => plugins.Remove(e.Plugin);

      using (AssemblyReflectionManager manager = new AssemblyReflectionManager())
      {
        string assemblyPath = GetType().Assembly.Location;
        manager.LoadAssembly(assemblyPath, Guid.NewGuid().ToString());
        mockSource.RaiseAssemblyAdded(new AssemblyAddedEventArgs(assemblyPath, manager));
      }

      PluginDescriptor plugin = plugins.First(x => x.QualifiedName == typeof(MockPlugin1));

      Assert.AreEqual(2, plugin.Settings.Count);
      Assert.IsTrue(plugin.Settings.Any(x => x.Name == "Setting" && x.SettingType == typeof(int)));
      Assert.IsTrue(plugin.Settings.Any(x => x.Name == "AnotherSetting" && x.SettingType == typeof(string)));
    }
    #endregion
  }
}
