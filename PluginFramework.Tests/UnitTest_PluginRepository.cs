using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PluginFramework.Tests.Mocks;
using PluginFramework.Logging;
using Moq;

namespace PluginFramework.Tests
{
  [TestClass]
  public sealed class UnitTest_PluginRepository
  {

    #region Construction
    [TestMethod]
    public void DefaultConstructor()
    {
      PluginRepository tested = new PluginRepository();
    }
    #endregion

    #region AddPluginSource
    [TestMethod]
    public void AddPluginSourceShouldRejectNull()
    {
      PluginRepository tested = new PluginRepository();
      DoAssert.Throws<ArgumentNullException>(() => tested.AddPluginSource(null) );
    }

    [TestMethod]
    public void AddPluginSourceShouldListenToAllEventsOnPluginSourceAfterAdd()
    {
      PluginRepository tested = new PluginRepository();
      MockPluginSource pluginSource = new MockPluginSource();
      tested.AddPluginSource(pluginSource);
      Assert.AreEqual(pluginSource.NumPluginAddedListeners, 1);
      Assert.AreEqual(pluginSource.NumPluginRemovedListeners, 1);
    }

    [TestMethod]
    public void AddPluginSourceCanOnlyAddOnce()
    {
      PluginRepository tested = new PluginRepository();
      MockPluginSource pluginSource = new MockPluginSource();
      tested.AddPluginSource(pluginSource);
      DoAssert.Throws<ArgumentException>(() => tested.AddPluginSource(pluginSource) );
    }
    #endregion

    #region RemovePluginSource
    [TestMethod]
    public void RemovePluginSourceShouldRejectNull()
    {
      PluginRepository tested = new PluginRepository();
      DoAssert.Throws<ArgumentNullException>(() => tested.RemovePluginSource(null) );
    }

    [TestMethod]
    public void RemovePluginSourceMustBeAddedToBeRemoved()
    {
      PluginRepository tested = new PluginRepository();
      MockPluginSource pluginSource = new MockPluginSource();
      DoAssert.Throws<ArgumentException>(() => tested.RemovePluginSource(pluginSource) );
    }

    [TestMethod]
    public void ShouldStopListeningToAllEventsOnPluginSourceAfterRemove()
    {
      PluginRepository tested = new PluginRepository();
      MockPluginSource pluginSource = new MockPluginSource();
      tested.AddPluginSource(pluginSource);
      tested.RemovePluginSource(pluginSource);
      Assert.AreEqual(pluginSource.NumPluginAddedListeners, 0);
      Assert.AreEqual(pluginSource.NumPluginRemovedListeners, 0);
    }
    #endregion

    #region OnPluginFound
    [TestMethod]
    public void RememberFoundPlugins()
    {
      PluginRepository tested = new PluginRepository();
      MockPluginSource pluginSource = new MockPluginSource();
      tested.AddPluginSource(pluginSource);

      var pluginsBefore = tested.Plugins(null).ToArray();
      PluginDescriptor thePlugin = new PluginDescriptor()
      {
        QualifiedName = typeof(UnitTest_PluginRepository)
      };
      
      pluginSource.RaisePluginAdded(thePlugin);

      var pluginsAfter = tested.Plugins(null).ToArray();

      Assert.IsTrue(pluginsAfter.Length - pluginsBefore.Length == 1);
      Assert.AreSame(thePlugin, pluginsAfter.Except(pluginsBefore).First());
    }
    #endregion

    #region OnPluginLost
    [TestMethod]
    public void ForgetLostPlugins()
    {
      PluginRepository tested = new PluginRepository();
      MockPluginSource pluginSource = new MockPluginSource();
      tested.AddPluginSource(pluginSource);

      PluginDescriptor thePlugin = new PluginDescriptor()
      {
        QualifiedName = typeof(UnitTest_PluginRepository)
      };
      
      pluginSource.RaisePluginAdded(thePlugin);

      var pluginsBefore = tested.Plugins(null).ToArray();
      pluginSource.RaisePluginRemoved(thePlugin);
      var pluginsAfter = tested.Plugins(null).ToArray();

      Assert.IsTrue(pluginsAfter.Length - pluginsBefore.Length == -1);
      Assert.IsFalse(pluginsAfter.Contains(thePlugin));
    }
    #endregion

    #region Plugin filtering
    [TestMethod]
    public void ShouldOnlyReportPluginsThatMatchesFilter()
    {
      PluginRepository tested = new PluginRepository();
      MockPluginSource pluginSource = new MockPluginSource();
      tested.AddPluginSource(pluginSource);

      PluginDescriptor plugin1 = new PluginDescriptor()
      {
        QualifiedName = typeof(MockPlugin1),
        Name = "plugin1"
      };

      PluginDescriptor plugin2 = new PluginDescriptor()
      {
        QualifiedName = typeof(MockPlugin2),
        Name = "plugin2"
      };

      pluginSource.RaisePluginAdded(plugin1);
      pluginSource.RaisePluginAdded(plugin2);

      PluginFilter filter = PluginFilter.Create.IsNamed("plugin1");
      var foundPlugins = tested.Plugins(filter);

      Assert.AreEqual(1, foundPlugins.Count());
      Assert.AreSame(plugin1, foundPlugins.First());
    }
    #endregion

    #region Logging
    [TestMethod]
    public void ImplementsILogWriter()
    {
      PluginRepository tested = new PluginRepository();
      Assert.IsInstanceOfType(tested, typeof(ILogWriter));
    }

    [TestMethod]
    public void ConstructorShouldInitLog()
    {
      ILogWriter tested = new PluginRepository();
      Assert.IsNotNull(tested.Log);
    }

    [TestMethod]
    public void ShouldLogAsInfoOnPluginAdded()
    {
      var mockPluginSource = new Mock<IPluginSource>();
      PluginRepository tested = new PluginRepository();
      tested.AddPluginSource(mockPluginSource.Object);
      MockLog log = new MockLog(tested);
      mockPluginSource.Raise(x => x.PluginAdded += null, new PluginEventArgs(MockPluginDescriptor.For<MockPlugin1>()));
      Assert.IsTrue(
        log.Any(
          x => x.Level == MockLog.Level.Info && 
          x.Message.Contains("Added") && 
          x.Message.Contains(typeof(MockPlugin1).FullName)));
    }

    [TestMethod]
    public void ShouldLogAsInfoOnPluginRemoved()
    {
      var mockPluginSource = new Mock<IPluginSource>();
      PluginRepository tested = new PluginRepository();
      tested.AddPluginSource(mockPluginSource.Object);
      MockLog log = new MockLog(tested);
      mockPluginSource.Raise(x => x.PluginRemoved += null, new PluginEventArgs(MockPluginDescriptor.For<MockPlugin1>()));
      Assert.IsTrue(
        log.Any(
          x => x.Level == MockLog.Level.Info &&
          x.Message.Contains("Removed") &&
          x.Message.Contains(typeof(MockPlugin1).FullName)));
    }

    [TestMethod]
    public void ShouldLogAsDebugWhenPluginSourceIsAdded()
    {
      PluginRepository tested = new PluginRepository();
      MockLog log = new MockLog(tested);
      var pluginSource = new Mock<IPluginSource>().Object;
      tested.AddPluginSource(pluginSource);
      Assert.IsTrue(log.Any(x => x.Level == MockLog.Level.Debug && x.Message.Contains("added") && x.Message.Contains(pluginSource.GetType().FullName)));
    }

    [TestMethod]
    public void ShouldLogAsDebugWhenPluginSourceIsRemoved()
    {
      PluginRepository tested = new PluginRepository();
      MockLog log = new MockLog(tested);
      var pluginSource = new Mock<IPluginSource>().Object;
      tested.AddPluginSource(pluginSource);
      tested.RemovePluginSource(pluginSource);
      Assert.IsTrue(log.Any(x => x.Level == MockLog.Level.Debug && x.Message.Contains("removed") && x.Message.Contains(pluginSource.GetType().FullName)));
    }

    [TestMethod]
    public void ShouldLogToDebugOnPluginQuery()
    {
      PluginRepository tested = new PluginRepository();
      MockLog log = new MockLog(tested);
      PluginFilter filter = PluginFilter.Create.IsNamed("plugin name").Implements(typeof(IMockPluginInterface1));
      tested.Plugins(filter);
      Assert.IsTrue(log.Any(x => x.Level == MockLog.Level.Debug && x.Message.Contains("Returning plugins for") && x.Message.Contains(filter.ToString())));
    }

    #endregion
  }
}
