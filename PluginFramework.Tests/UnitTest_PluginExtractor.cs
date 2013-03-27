using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PluginFramework.Tests.Mocks;
using PluginFramework.Logging;
using Moq;
using System.Text.RegularExpressions;

namespace PluginFramework.Tests
{
  [TestClass]
  public class UnitTest_PluginExtractor
  {
    #region Construction
    [TestMethod]
    public void ConstructionRequiresIAssemblySource()
    {
      DoAssert.Throws<ArgumentNullException>(() => new PluginExtractor(null));
    }

    [TestMethod]
    public void ConstructionWithIAssemblySource()
    {
      MockAssemblySource mockSource = new MockAssemblySource();
      PluginExtractor tested = new PluginExtractor(mockSource);
      Assert.IsNotNull(tested);
    }
    #endregion

    #region OnAssemblyAdded
    [TestMethod]
    public void OnAssemblyAddedShouldRaisePluginFoundForEachPluginInAssembly()
    {
      MockAssemblySource mockSource = new MockAssemblySource();
      PluginExtractor tested = new PluginExtractor(mockSource);

      List<PluginDescriptor> plugins = new List<PluginDescriptor>();
      tested.PluginAdded += (s, e) => plugins.Add(e.Plugin);

      using (AssemblyReflectionManager manager = new AssemblyReflectionManager())
      {
        string assemblyPath = GetType().Assembly.Location;
        manager.LoadAssembly(assemblyPath);
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
      PluginExtractor tested = new PluginExtractor(mockSource);

      List<PluginDescriptor> plugins = new List<PluginDescriptor>();
      tested.PluginAdded += (s, e) => plugins.Add(e.Plugin);
      tested.PluginRemoved += (s, e) => plugins.Remove(e.Plugin);

      using (AssemblyReflectionManager manager = new AssemblyReflectionManager())
      {
        string assemblyPath = GetType().Assembly.Location;
        manager.LoadAssembly(assemblyPath);
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
      PluginExtractor tested = new PluginExtractor(mockSource);

      List<PluginDescriptor> plugins = new List<PluginDescriptor>();
      tested.PluginAdded += (s, e) => plugins.Add(e.Plugin);
      tested.PluginRemoved += (s, e) => plugins.Remove(e.Plugin);

      using (AssemblyReflectionManager manager = new AssemblyReflectionManager())
      {
        string assemblyPath = GetType().Assembly.Location;
        manager.LoadAssembly(assemblyPath);
        mockSource.RaiseAssemblyAdded(new AssemblyAddedEventArgs(assemblyPath, manager));
      }

      var expected = typeof(MockPlugin1).FullName;
      PluginDescriptor plugin = plugins.First(x => x.QualifiedName == typeof(MockPlugin1));

      Assert.AreEqual(expected, plugin.Name);
    }

    [TestMethod]
    public void PluginDescriptorShouldContainSpecifiedName()
    {
      MockAssemblySource mockSource = new MockAssemblySource();
      PluginExtractor tested = new PluginExtractor(mockSource);

      List<PluginDescriptor> plugins = new List<PluginDescriptor>();
      tested.PluginAdded += (s, e) => plugins.Add(e.Plugin);
      tested.PluginRemoved += (s, e) => plugins.Remove(e.Plugin);

      using (AssemblyReflectionManager manager = new AssemblyReflectionManager())
      {
        string assemblyPath = GetType().Assembly.Location;
        manager.LoadAssembly(assemblyPath);
        mockSource.RaiseAssemblyAdded(new AssemblyAddedEventArgs(assemblyPath, manager));
      }

      PluginDescriptor plugin = plugins.First(x => x.QualifiedName == typeof(MockPlugin2));

      Assert.AreEqual("MockPlugin2", plugin.Name);
    }

    [TestMethod]
    public void PluginDescriptorShouldContainDerivedClasses()
    {
      MockAssemblySource mockSource = new MockAssemblySource();
      PluginExtractor tested = new PluginExtractor(mockSource);

      List<PluginDescriptor> plugins = new List<PluginDescriptor>();
      tested.PluginAdded += (s, e) => plugins.Add(e.Plugin);
      tested.PluginRemoved += (s, e) => plugins.Remove(e.Plugin);

      using (AssemblyReflectionManager manager = new AssemblyReflectionManager())
      {
        string assemblyPath = GetType().Assembly.Location;
        manager.LoadAssembly(assemblyPath);
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
      PluginExtractor tested = new PluginExtractor(mockSource);

      List<PluginDescriptor> plugins = new List<PluginDescriptor>();
      tested.PluginAdded += (s, e) => plugins.Add(e.Plugin);
      tested.PluginRemoved += (s, e) => plugins.Remove(e.Plugin);

      using (AssemblyReflectionManager manager = new AssemblyReflectionManager())
      {
        string assemblyPath = GetType().Assembly.Location;
        manager.LoadAssembly(assemblyPath);
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
      PluginExtractor tested = new PluginExtractor(mockSource);

      List<PluginDescriptor> plugins = new List<PluginDescriptor>();
      tested.PluginAdded += (s, e) => plugins.Add(e.Plugin);
      tested.PluginRemoved += (s, e) => plugins.Remove(e.Plugin);

      using (AssemblyReflectionManager manager = new AssemblyReflectionManager())
      {
        string assemblyPath = GetType().Assembly.Location;
        manager.LoadAssembly(assemblyPath);
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
      PluginExtractor tested = new PluginExtractor(mockSource);

      List<PluginDescriptor> plugins = new List<PluginDescriptor>();
      tested.PluginAdded += (s, e) => plugins.Add(e.Plugin);
      tested.PluginRemoved += (s, e) => plugins.Remove(e.Plugin);

      using (AssemblyReflectionManager manager = new AssemblyReflectionManager())
      {
        string assemblyPath = GetType().Assembly.Location;
        manager.LoadAssembly(assemblyPath);
        mockSource.RaiseAssemblyAdded(new AssemblyAddedEventArgs(assemblyPath, manager));
      }

      PluginDescriptor plugin = plugins.First(x => x.QualifiedName == typeof(MockPlugin1));

      Assert.AreEqual(2, plugin.Settings.Count);
      Assert.IsTrue(plugin.Settings.Any(x => x.Name == "Setting" && x.SettingType == typeof(int)));
      Assert.IsTrue(plugin.Settings.Any(x => x.Name == "AnotherSetting" && x.SettingType == typeof(string)));
    }
    #endregion

    #region Logging
    [TestMethod]
    public void ShouldImplementILogWriter()
    {
      PluginExtractor tested = new PluginExtractor(new Mock<IAssemblySource>().Object);
      Assert.IsInstanceOfType(tested, typeof(ILogWriter));
    }

    [TestMethod]
    public void ConstructorShouldInitLog()
    {
      ILogWriter tested = new PluginExtractor(new Mock<IAssemblySource>().Object);
      Assert.IsNotNull(tested.Log);
    }

    [TestMethod]
    public void ShouldLogToInfoNumberOfAddedPluginsForAssembly()
    {
      var path = GetType().Assembly.Location;
      var pattern = new Regex(@"^Found \d+ plugins in .+$");
      var mockAssemblySource = new Mock<IAssemblySource>();
      PluginExtractor tested = new PluginExtractor(mockAssemblySource.Object);
      MockLog log = new MockLog(tested);
      using (AssemblyReflectionManager manager = new AssemblyReflectionManager())
      {
        manager.LoadAssembly(path);
        mockAssemblySource.Raise(x => x.AssemblyAdded += null, new AssemblyAddedEventArgs(path, manager));
      }
      Assert.IsTrue(log.Any(x => x.Level == MockLog.Level.Info && pattern.IsMatch(x.Message) && x.Message.Contains(path)));
    }

    [TestMethod]
    public void ShouldLogToInfoNumberOfLostPluginsForAssembly()
    {
      var path = GetType().Assembly.Location;
      var pattern = new Regex(@"^Lost \d+ plugins when .+ was removed$");
      var mockAssemblySource = new Mock<IAssemblySource>();
      PluginExtractor tested = new PluginExtractor(mockAssemblySource.Object);
      MockLog log = new MockLog(tested);
      using (AssemblyReflectionManager manager = new AssemblyReflectionManager())
      {
        manager.LoadAssembly(path);
        mockAssemblySource.Raise(x => x.AssemblyAdded += null, new AssemblyAddedEventArgs(path, manager));
        mockAssemblySource.Raise(x => x.AssemblyRemoved += null, new AssemblyRemovedEventArgs(path));
      }
      Assert.IsTrue(log.Any(x => x.Level == MockLog.Level.Info && pattern.IsMatch(x.Message) && x.Message.Contains(path)));
    }

    #endregion
  }
}
