using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace PluginFramework.Tests
{
  [TestClass]
  public class UnitTest_SafeEventFileSystemWatcher
  {
    #region Constructing
    [TestMethod]
    public void RequiresInternalWatcher()
    {
      DoAssert.Throws<ArgumentNullException>(() => new SafeEventFileSystemWatcher(null));
    }

    [TestMethod]
    public void ShouldListenToAllEventsOnInternalWatcher()
    {
      MockFileSystemWatcher mockWatcher = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockWatcher);

      Assert.AreEqual(1, mockWatcher.CreatedListeners().Length);
      Assert.AreEqual(1, mockWatcher.ChangedListeners().Length);
      Assert.AreEqual(1, mockWatcher.DeletedListeners().Length);
      Assert.AreEqual(1, mockWatcher.RenamedListeners().Length);
    }
    #endregion

    #region Path
    [TestMethod]
    public void ShouldExposePathFromInternal()
    {
      MockFileSystemWatcher mockWatcher = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockWatcher);

      mockWatcher.Path = "FAKEPATH";
      Assert.AreEqual("FAKEPATH", tested.Path);
    }

    [TestMethod]
    public void ShouldUpdatePathOnInternal()
    {
      MockFileSystemWatcher mockWatcher = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockWatcher);

      tested.Path = "FAKEPATH";
      Assert.AreEqual("FAKEPATH", mockWatcher.Path);
    }
    #endregion

    #region Filter
    [TestMethod]
    public void ShouldExposeFilterFromInternal()
    {
      MockFileSystemWatcher mockWatcher = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockWatcher);

      mockWatcher.Filter = "FAKEFILTER";
      Assert.AreEqual("FAKEFILTER", tested.Filter);
    }

    [TestMethod]
    public void ShouldUpdateFilterOnInternal()
    {
      MockFileSystemWatcher mockWatcher = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockWatcher);

      tested.Filter = "FAKEFILTER";
      Assert.AreEqual("FAKEFILTER", mockWatcher.Filter);
    }
    #endregion

    #region EnableRaisingEvents
    [TestMethod]
    public void ShouldExposeEnableRaisingEventsFromInternal()
    {
      MockFileSystemWatcher mockWatcher = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockWatcher);

      mockWatcher.EnableRaisingEvents = true;
      Assert.AreEqual(true, tested.EnableRaisingEvents);
      mockWatcher.EnableRaisingEvents = false;
      Assert.AreEqual(false, tested.EnableRaisingEvents);
    }

    [TestMethod]
    public void ShouldUpdateEnableRaisingEventsOnInternal()
    {
      MockFileSystemWatcher mockWatcher = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockWatcher);

      tested.EnableRaisingEvents = false;
      Assert.AreEqual(false, mockWatcher.EnableRaisingEvents);
      tested.EnableRaisingEvents = true;
      Assert.AreEqual(true, mockWatcher.EnableRaisingEvents);
    }
    #endregion

    #region IncludeSubdirectories
    [TestMethod]
    public void ShouldExposeIncludeSubdirectoriesFromInternal()
    {
      MockFileSystemWatcher mockWatcher = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockWatcher);

      mockWatcher.IncludeSubdirectories = true;
      Assert.AreEqual(true, tested.IncludeSubdirectories);
      mockWatcher.IncludeSubdirectories = false;
      Assert.AreEqual(false, tested.IncludeSubdirectories);
    }

    [TestMethod]
    public void ShouldUpdateIncludeSubdirectoriesOnInternal()
    {
      MockFileSystemWatcher mockWatcher = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockWatcher);

      tested.IncludeSubdirectories = false;
      Assert.AreEqual(false, mockWatcher.IncludeSubdirectories);
      tested.IncludeSubdirectories = true;
      Assert.AreEqual(true, mockWatcher.IncludeSubdirectories);
    }
    #endregion

    #region NotifyFilter
    [TestMethod]
    public void ShouldExposeNotifyFilterFromInternal()
    {
      MockFileSystemWatcher mockWatcher = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockWatcher);

      mockWatcher.NotifyFilter = NotifyFilters.LastAccess;
      Assert.AreEqual(NotifyFilters.LastAccess, tested.NotifyFilter);
      mockWatcher.NotifyFilter = NotifyFilters.DirectoryName;
      Assert.AreEqual(NotifyFilters.DirectoryName, tested.NotifyFilter);
    }

    [TestMethod]
    public void ShouldUpdateNotifyFilterOnInternal()
    {
      MockFileSystemWatcher mockWatcher = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockWatcher);

      tested.NotifyFilter = NotifyFilters.DirectoryName;
      Assert.AreEqual(NotifyFilters.DirectoryName, mockWatcher.NotifyFilter);
      tested.NotifyFilter = NotifyFilters.LastAccess;
      Assert.AreEqual(NotifyFilters.LastAccess, mockWatcher.NotifyFilter);
    }
    #endregion

    #region RaiseEvent
    [TestMethod]
    public void RaiseEventRequiresArgument()
    {
      MockFileSystemWatcher mockWatcher = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockWatcher);
      DoAssert.Throws<ArgumentNullException>(() => tested.RaiseEvent(null));
    }

    [TestMethod]
    public void RaseEventThrowsArgumentExceptionOnUnknownEvent()
    {
      MockFileSystemWatcher mockWatcher = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockWatcher);
      var evt = new PluginFramework.SafeEventFileSystemWatcher.WatcherEvent((SafeEventFileSystemWatcher.WatcherEventType)666, null);
      DoAssert.Throws<ArgumentException>(() => tested.RaiseEvent(evt));
    }
    #endregion

    #region event Created
    [TestMethod]
    public void ShouldRaiseCreatedIfInternalRaiseCreated()
    {
      AutoResetEvent raisedEvent = new AutoResetEvent(false);
      MockFileSystemWatcher mockInternal = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockInternal);
      int CreatedRaised = 0;
      tested.Created += (s, e) =>
      {
        CreatedRaised++;
        raisedEvent.Set();
      };

      string fileName = Guid.NewGuid().ToString();
      try
      {
        using (var file = System.IO.File.CreateText(fileName))
          file.WriteLine("somedata");

        mockInternal.RaiseCreated(fileName);
        raisedEvent.WaitOne(TimeSpan.FromSeconds(1));

        Assert.AreEqual(1, CreatedRaised);
      }
      finally
      {
        System.IO.File.Delete(fileName);
      }
    }

    [TestMethod]
    public void ShouldWaitRaisingCreatedUntilFileIsUnlocked()
    {
      AutoResetEvent raisedEvent = new AutoResetEvent(false);
      MockFileSystemWatcher mockInternal = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockInternal);
      int CreatedRaised = 0;
      tested.Created += (s, e) =>
      {
        CreatedRaised++;
        raisedEvent.Set();
      };

      string fileName = Guid.NewGuid().ToString();
      try
      {
        using (var file = System.IO.File.CreateText(fileName))
        {
          file.WriteLine("somedata");

          mockInternal.RaiseCreated(fileName);
          
          raisedEvent.WaitOne(500);

          Assert.AreEqual(0, CreatedRaised);
        }

        raisedEvent.WaitOne(500);

        Assert.AreEqual(1, CreatedRaised);
      }
      finally
      {
        System.IO.File.Delete(fileName);
      }
    }
    #endregion

    #region event Changed
    [TestMethod]
    public void ShouldRaiseChangedIfInternalRaisesChanged()
    {
      AutoResetEvent raisedEvent = new AutoResetEvent(false);
      MockFileSystemWatcher mockInternal = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockInternal);
      int ChangedRaised = 0;
      tested.Changed += (s, e) =>
      {
        ChangedRaised++;
        raisedEvent.Set();
      };

      string fileName = Guid.NewGuid().ToString();
      try
      {
        using (var file = System.IO.File.CreateText(fileName))
          file.WriteLine("somedata");

        mockInternal.RaiseChanged(fileName);
        raisedEvent.WaitOne(TimeSpan.FromSeconds(1));

        Assert.AreEqual(1, ChangedRaised);
      }
      finally
      {
        System.IO.File.Delete(fileName);
      }
    }

    [TestMethod]
    public void ShouldWaitRaisingChangedUntilFileIsUnlocked()
    {
      AutoResetEvent raisedEvent = new AutoResetEvent(false);
      MockFileSystemWatcher mockInternal = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockInternal);
      int ChangedRaised = 0;
      tested.Changed += (s, e) =>
      {
        ChangedRaised++;
        raisedEvent.Set();
      };

      string fileName = Guid.NewGuid().ToString();
      try
      {
        using (var file = System.IO.File.CreateText(fileName))
        {
          file.WriteLine("somedata");

          mockInternal.RaiseChanged(fileName);
          raisedEvent.WaitOne(TimeSpan.FromSeconds(1));
          Thread.Sleep(500);
          Assert.AreEqual(0, ChangedRaised);
        }
        raisedEvent.WaitOne(TimeSpan.FromSeconds(1));
        Assert.AreEqual(1, ChangedRaised);
      }
      finally
      {
        System.IO.File.Delete(fileName);
      }
    }

    [TestMethod]
    public void ShouldNotRaiseChangedIfPendingRaiseCreated()
    {
      MockFileSystemWatcher mockInternal = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockInternal);
      int CreatedRaised = 0;
      int ChangedRaised = 0;
      tested.Created += (s, e) =>
      {
        CreatedRaised++;
      };
      tested.Changed += (s, e) =>
      {
        ChangedRaised++;
      };

      string fileName = Guid.NewGuid().ToString();
      try
      {
        using (var file = System.IO.File.CreateText(fileName))
        {
          file.WriteLine("somedata");

          mockInternal.RaiseCreated(fileName);
          mockInternal.RaiseChanged(fileName);
          Thread.Sleep(500);
          Assert.AreEqual(0, CreatedRaised);
          Assert.AreEqual(0, ChangedRaised);
        }
        Thread.Sleep(500);
        Assert.AreEqual(1, CreatedRaised);
        Assert.AreEqual(0, ChangedRaised);
      }
      finally
      {
        System.IO.File.Delete(fileName);
      }
    }

    [TestMethod]
    public void ShouldOnlyRaiseChangedOnceIfSeveralIsPending()
    {
      MockFileSystemWatcher mockInternal = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockInternal);
      int ChangedRaised = 0;
      tested.Changed += (s, e) => ChangedRaised++;

      string fileName = Guid.NewGuid().ToString();
      try
      {
        using (var file = System.IO.File.CreateText(fileName))
        {
          file.WriteLine("somedata");
          mockInternal.RaiseChanged(fileName);
          mockInternal.RaiseChanged(fileName);
          mockInternal.RaiseChanged(fileName); 
        }
        Thread.Sleep(500);
        Assert.AreEqual(1, ChangedRaised);
      }
      finally
      {
        System.IO.File.Delete(fileName);
      }
    }
    #endregion

    #region event Deleted
    [TestMethod]
    public void ShouldRaiseDeletedIfInternalRaisesDeleted()
    {
      AutoResetEvent raisedEvent = new AutoResetEvent(false);
      MockFileSystemWatcher mockInternal = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockInternal);
      int DeletedRaised = 0;
      tested.Deleted += (s, e) =>
      {
        DeletedRaised++;
        raisedEvent.Set();
      };

      string fileName = Guid.NewGuid().ToString();
      try
      {
        using (var file = System.IO.File.CreateText(fileName))
          file.WriteLine("somedata");

        mockInternal.RaiseDeleted(fileName);
        raisedEvent.WaitOne(TimeSpan.FromSeconds(1));

        Assert.AreEqual(1, DeletedRaised);
      }
      finally
      {
        System.IO.File.Delete(fileName);
      }
    }

    [TestMethod]
    public void ShouldNotEmitAnythingIfDeletedWhileStillPendingRaiseCreated()
    {
      MockFileSystemWatcher mockInternal = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockInternal);
      int CreatedRaised = 0;
      int ChangedRaised = 0;
      int DeletedRaised = 0;
      tested.Created += (s, e) => CreatedRaised++;
      tested.Changed += (s, e) => ChangedRaised++;
      tested.Deleted += (s, e) => DeletedRaised++;

      string fileName = Guid.NewGuid().ToString();
      try
      {
        using (var file = System.IO.File.CreateText(fileName))
        {
          file.WriteLine("somedata");

          mockInternal.RaiseCreated(fileName);
          mockInternal.RaiseChanged(fileName);
          Thread.Sleep(500);
          Assert.AreEqual(0, CreatedRaised);
          Assert.AreEqual(0, ChangedRaised);
        }
        System.IO.File.Delete(fileName);
        mockInternal.RaiseDeleted(fileName);
        Thread.Sleep(500);

        Assert.AreEqual(0, CreatedRaised);
        Assert.AreEqual(0, ChangedRaised);
        Assert.AreEqual(0, DeletedRaised);
      }
      finally
      {
        System.IO.File.Delete(fileName);
      }
    }

    [TestMethod]
    public void ShouldNotRaiseEventsOnNonExistingFileUntilPendingDeleted()
    {
      MockFileSystemWatcher mockInternal = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockInternal);
      int ChangedRaised = 0;
      int DeletedRaised = 0;
      tested.Changed += (s, e) => ChangedRaised++;
      tested.Deleted += (s, e) => DeletedRaised++;

      string fileName = Guid.NewGuid().ToString();
      mockInternal.RaiseChanged(fileName);
      Thread.Sleep(500);
      Assert.AreEqual(0, ChangedRaised);

      mockInternal.RaiseDeleted(fileName);
      Thread.Sleep(500);

      Assert.AreEqual(1, ChangedRaised);
      Assert.AreEqual(1, DeletedRaised);
    }
    #endregion

    #region event Renamed
    [TestMethod]
    public void ShouldRaiseRenamedIfInternalRaisesRenamed()
    {
      AutoResetEvent raisedEvent = new AutoResetEvent(false);
      MockFileSystemWatcher mockInternal = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockInternal);
      int RenamedRaised = 0;
      tested.Renamed += (s, e) =>
      {
        RenamedRaised++;
        raisedEvent.Set();
      };

      string fileName = Guid.NewGuid().ToString();
      string newFileName = Guid.NewGuid().ToString();
      try
      {
        using (var file = System.IO.File.CreateText(newFileName))
          file.WriteLine("somedata");

        mockInternal.RaiseRenamed(fileName, newFileName);
        raisedEvent.WaitOne(TimeSpan.FromSeconds(1));

        Assert.AreEqual(1, RenamedRaised);
      }
      finally
      {
        System.IO.File.Delete(fileName);
      }
    }

    [TestMethod]
    public void ShouldDelayRaiseRenamedUntilFileIsUnlocked()
    {
      AutoResetEvent raisedEvent = new AutoResetEvent(false);
      MockFileSystemWatcher mockInternal = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockInternal);
      int RenamedRaised = 0;
      tested.Renamed += (s, e) =>
      {
        RenamedRaised++;
        raisedEvent.Set();
      };

      string fileName = Guid.NewGuid().ToString();
      string newFileName = Guid.NewGuid().ToString();
      try
      {
        using (var file = System.IO.File.CreateText(newFileName))
        {
          file.WriteLine("somedata");

          mockInternal.RaiseRenamed(fileName, newFileName);
          Assert.AreEqual(0, RenamedRaised);
          raisedEvent.WaitOne(500);
        }
        raisedEvent.WaitOne(500);
        Assert.AreEqual(1, RenamedRaised);
      }
      finally
      {
        System.IO.File.Delete(fileName);
      }
    }
    #endregion

    #region Dispose
    [TestMethod]
    public void DisposeShouldRaiseDisposed()
    {
      MockFileSystemWatcher mockInternal = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockInternal);
      bool wasDisposed = false;
      tested.Disposed += (s, e) => wasDisposed = true;
      tested.Dispose();
      Assert.IsTrue(wasDisposed);
    }

    [TestMethod]
    public void DisposeShouldDisposeInnerFileSystemWatcher()
    {
      MockFileSystemWatcher mockInternal = new MockFileSystemWatcher();
      SafeEventFileSystemWatcher tested = new SafeEventFileSystemWatcher(mockInternal);

      bool wasDisposed = false;
      mockInternal.Disposed += (s,e) => wasDisposed = true;
      tested.Dispose();
      Assert.IsTrue(wasDisposed);
    }
    #endregion
  }
}
