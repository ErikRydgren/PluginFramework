using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using PluginFramework.Logging;
using PluginFramework.Tests.Mocks;

namespace PluginFramework.Tests
{
  [TestClass]
  public class UnitTest_FileSystemWatcher
  {
    #region Constructing
    [TestMethod]
    public void ConstructingDefault()
    {
      FileSystemWatcher tested = new FileSystemWatcher();
      Assert.IsNotNull(tested);
    }

    [TestMethod]
    public void ConstructingWithPath()
    {
      FileSystemWatcher tested = new FileSystemWatcher(AppDomain.CurrentDomain.BaseDirectory);
      Assert.IsNotNull(tested);
      Assert.AreEqual(AppDomain.CurrentDomain.BaseDirectory, tested.Path);
    }

    [TestMethod]
    public void ConstructingWithPathAndFilter()
    {
      FileSystemWatcher tested = new FileSystemWatcher(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
      Assert.IsNotNull(tested);
      Assert.AreEqual(AppDomain.CurrentDomain.BaseDirectory, tested.Path);
      Assert.AreEqual("*.dll", tested.Filter);
    }

    [TestMethod]
    public void ConstructingShouldNotEnableEvents()
    {
      FileSystemWatcher tested = new FileSystemWatcher();
      Assert.IsFalse(tested.EnableRaisingEvents);
    }
    #endregion

    #region Events
    [TestMethod]
    public void RaisesCreatedOnNewFile()
    {
      System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(".");
      string fileName = System.IO.Path.Combine(dir.FullName, Guid.NewGuid().ToString());

      try
      {
        AutoResetEvent raised = new AutoResetEvent(false);

        FileSystemWatcher tested = new FileSystemWatcher(dir.FullName);
        string raisedFileName = null;
        tested.Created += (s, e) =>
          {
            raisedFileName = e.FullPath;
            raised.Set();
          };
        tested.EnableRaisingEvents = true;

        using (System.IO.FileStream file = System.IO.File.Create(fileName))
        {
          byte[] data = Encoding.UTF8.GetBytes("somedata");
          file.Write(data, 0, data.Length);
          file.Flush();
          file.Close();
        };

        raised.WaitOne(TimeSpan.FromSeconds(1));

        Assert.IsNotNull(raisedFileName);
        Assert.AreEqual(fileName, raisedFileName);
      }
      finally
      {
        System.IO.File.Delete(fileName);
      }
    }

    [TestMethod]
    public void RaisesChangedOnFileChange()
    {
      System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(".");
      string fileName = System.IO.Path.Combine(dir.FullName, Guid.NewGuid().ToString());
      try
      {
        AutoResetEvent raised = new AutoResetEvent(false);
        using (System.IO.FileStream file = System.IO.File.Create(fileName))
        {
          byte[] data = Encoding.UTF8.GetBytes("somedata");
          file.Write(data, 0, data.Length);
          file.Flush();
          file.Close();
        };

        FileSystemWatcher tested = new FileSystemWatcher(dir.FullName);
        string raisedFileName = null;
        tested.Changed += (s, e) =>
        {
          raisedFileName = e.FullPath;
          raised.Set();
        };
        tested.EnableRaisingEvents = true;

        using (System.IO.FileStream file = System.IO.File.OpenWrite(fileName))
        {
          byte[] data = Encoding.UTF8.GetBytes("somedata");
          file.Write(data, 0, data.Length);
          file.Flush();
          file.Close();
        };        

        raised.WaitOne(TimeSpan.FromSeconds(1));

        Assert.IsNotNull(raisedFileName);
        Assert.AreEqual(fileName, raisedFileName);
      }
      finally
      {
        System.IO.File.Delete(fileName);
      }
    }

    [TestMethod]
    public void RaisesDeletedOnFileDelete()
    {
      System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(".");
      string fileName = System.IO.Path.Combine(dir.FullName, Guid.NewGuid().ToString());
      try
      {
        AutoResetEvent raised = new AutoResetEvent(false);
        using (System.IO.FileStream file = System.IO.File.Create(fileName))
        {
          byte[] data = Encoding.UTF8.GetBytes("somedata");
          file.Write(data, 0, data.Length);
          file.Flush();
          file.Close();
        };

        FileSystemWatcher tested = new FileSystemWatcher(dir.FullName);
        string raisedFileName = null;
        tested.Deleted += (s, e) =>
        {
          raisedFileName = e.FullPath;
          raised.Set();
        };
        tested.EnableRaisingEvents = true;

        System.IO.File.Delete(fileName);

        raised.WaitOne(TimeSpan.FromSeconds(1));

        Assert.IsNotNull(raisedFileName);
        Assert.AreEqual(fileName, raisedFileName);
      }
      finally
      {
        System.IO.File.Delete(fileName);        
      }
    }

    [TestMethod]
    public void RaisesRenamedOnFileRename()
    {
      System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(".");
      string fileName = System.IO.Path.Combine(dir.FullName, Guid.NewGuid().ToString());
      string newFileName = System.IO.Path.Combine(dir.FullName, Guid.NewGuid().ToString());
      try
      {
        AutoResetEvent raised = new AutoResetEvent(false);
        using (System.IO.FileStream file = System.IO.File.Create(fileName))
        {
          byte[] data = Encoding.UTF8.GetBytes("somedata");
          file.Write(data, 0, data.Length);
          file.Flush();
          file.Close();
        };

        FileSystemWatcher tested = new FileSystemWatcher(dir.FullName);
        string raisedFileName = null;
        string raisedNewFileName = null;
        tested.Renamed += (s, e) =>
        {
          raisedFileName = e.OldFullPath;
          raisedNewFileName = e.FullPath;
          raised.Set();
        };
        tested.EnableRaisingEvents = true;

        System.IO.File.Move(fileName, newFileName);

        raised.WaitOne(TimeSpan.FromSeconds(1));

        Assert.IsNotNull(raisedFileName);
        Assert.IsNotNull(raisedNewFileName);
        Assert.AreEqual(fileName, raisedFileName);
        Assert.AreEqual(newFileName, raisedNewFileName);
      }
      finally
      {
        System.IO.File.Delete(fileName);
        System.IO.File.Delete(newFileName);
      }
    }
    #endregion

    #region Path
    [TestMethod]
    public void ShouldBeAbleToChangePath()
    {
      FileSystemWatcher tested = new FileSystemWatcher(".");
      string oldPath = tested.Path;
      tested.Path = AppDomain.CurrentDomain.BaseDirectory;
      string newPath = tested.Path;

      Assert.AreNotEqual(oldPath, newPath);
      Assert.AreEqual(AppDomain.CurrentDomain.BaseDirectory, newPath);
    }
    #endregion

    #region EnableRaisingEvents
    [TestMethod]
    public void NoEventsAreRaisedIfEnableRaisingEventsAreFalse()
    {
      System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(".");
      string fileName = System.IO.Path.Combine(dir.FullName, Guid.NewGuid().ToString());
      string newFileName = System.IO.Path.Combine(dir.FullName, Guid.NewGuid().ToString());
      try
      {
        AutoResetEvent raised = new AutoResetEvent(false);
        FileSystemWatcher tested = new FileSystemWatcher(dir.FullName);
        tested.EnableRaisingEvents = false;
        int CreateRaised = 0;
        int ChangedRaised = 0;
        int DeletedRaised = 0;
        int RenamedRaised = 0;
        tested.Created += (s, e) => CreateRaised++;
        tested.Changed += (s, e) => ChangedRaised++;
        tested.Deleted += (s, e) => DeletedRaised++;
        tested.Renamed += (s, e) => RenamedRaised++;

        using (System.IO.FileStream file = System.IO.File.Create(fileName))
        {
          byte[] data = Encoding.UTF8.GetBytes("somedata");
          file.Write(data, 0, data.Length);
          file.Flush();
          file.Close();
        };
        Thread.Sleep(100);
        using (System.IO.FileStream file = System.IO.File.OpenWrite(fileName))
        {
          byte[] data = Encoding.UTF8.GetBytes("somedata");
          file.Write(data, 0, data.Length);
          file.Flush();
          file.Close();
        };
        Thread.Sleep(100);
        System.IO.File.Move(fileName, newFileName);
        Thread.Sleep(100);
        System.IO.File.Delete(newFileName);
        Thread.Sleep(100);

        Assert.AreEqual(0, CreateRaised);
        Assert.AreEqual(0, ChangedRaised);
        Assert.AreEqual(0, DeletedRaised);
        Assert.AreEqual(0, RenamedRaised);
      }
      finally
      {
        System.IO.File.Delete(fileName);
        System.IO.File.Delete(newFileName);
      }
    } 
    #endregion

    #region IncludeSubdirectories
    [TestMethod]
    public void RememberSettingForIncludeSubdirectories()
    {
      FileSystemWatcher tested = new FileSystemWatcher();
      bool oldValue = tested.IncludeSubdirectories;
      tested.IncludeSubdirectories = !oldValue;
      Assert.AreNotEqual(oldValue, tested.IncludeSubdirectories);
    }

    [TestMethod]
    public void SubdirectoriedAreWachedIfIncludeSubdirIsTrue()
    {
      System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(".");
      System.IO.DirectoryInfo subdir = dir.CreateSubdirectory(Guid.NewGuid().ToString());
      string fileName = System.IO.Path.Combine(subdir.FullName, Guid.NewGuid().ToString());
      string newFileName = System.IO.Path.Combine(subdir.FullName, Guid.NewGuid().ToString());
      try
      {
        AutoResetEvent raised = new AutoResetEvent(false);
        FileSystemWatcher tested = new FileSystemWatcher(dir.FullName);
        tested.IncludeSubdirectories = true;
        tested.EnableRaisingEvents = true;
        int CreateRaised = 0;
        int ChangedRaised = 0;
        int DeletedRaised = 0;
        int RenamedRaised = 0;
        tested.Created += (s, e) => CreateRaised++;
        tested.Changed += (s, e) => ChangedRaised++;
        tested.Deleted += (s, e) => DeletedRaised++;
        tested.Renamed += (s, e) => RenamedRaised++;

        using (System.IO.FileStream file = System.IO.File.Create(fileName))
        {
          byte[] data = Encoding.UTF8.GetBytes("somedata");
          file.Write(data, 0, data.Length);
          file.Flush();
          file.Close();
        };
        Thread.Sleep(200);
        using (System.IO.FileStream file = System.IO.File.OpenWrite(fileName))
        {
          byte[] data = Encoding.UTF8.GetBytes("somedata");
          file.Write(data, 0, data.Length);
          file.Flush();
          file.Close();
        };
        Thread.Sleep(200);
        System.IO.File.Move(fileName, newFileName);
        Thread.Sleep(200);
        System.IO.File.Delete(newFileName);
        Thread.Sleep(200);

        Assert.AreEqual(1, CreateRaised);
        Assert.IsTrue(ChangedRaised > 0);
        Assert.AreEqual(1, DeletedRaised);
        Assert.AreEqual(1, RenamedRaised);
      }
      finally
      {
        subdir.Delete(true);
      }
    }

    [TestMethod]
    public void SubdirectoriedAreNotWachedIfIncludeSubdirIsFalse()
    {
      System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(".");
      System.IO.DirectoryInfo subdir = dir.CreateSubdirectory(Guid.NewGuid().ToString());
      string fileName = System.IO.Path.Combine(subdir.FullName, Guid.NewGuid().ToString());
      string newFileName = System.IO.Path.Combine(subdir.FullName, Guid.NewGuid().ToString());
      try
      {
        AutoResetEvent raised = new AutoResetEvent(false);
        FileSystemWatcher tested = new FileSystemWatcher(dir.FullName);
        tested.IncludeSubdirectories = false;        
        tested.EnableRaisingEvents = true;
        int CreateRaised = 0;
        int ChangedRaised = 0;
        int DeletedRaised = 0;
        int RenamedRaised = 0;
        tested.Created += (s, e) => CreateRaised++;
        tested.Changed += (s, e) =>
        {
          // Ignore changes for the subdir (it resides inside the watched dir)
          if (e.FullPath == subdir.FullName)
            return;

          ChangedRaised++;
        };
        tested.Deleted += (s, e) => DeletedRaised++;
        tested.Renamed += (s, e) => RenamedRaised++;

        using (System.IO.FileStream file = System.IO.File.Create(fileName))
        {
          byte[] data = Encoding.UTF8.GetBytes("somedata");
          file.Write(data, 0, data.Length);
          file.Flush();
          file.Close();
        };
        Thread.Sleep(100);
        using (System.IO.FileStream file = System.IO.File.OpenWrite(fileName))
        {
          byte[] data = Encoding.UTF8.GetBytes("somedata");
          file.Write(data, 0, data.Length);
          file.Flush();
          file.Close();
        };
        Thread.Sleep(100);
        System.IO.File.Move(fileName, newFileName);
        Thread.Sleep(100);
        System.IO.File.Delete(newFileName);
        Thread.Sleep(100);

        Assert.AreEqual(0, CreateRaised);
        Assert.AreEqual(0, ChangedRaised);
        Assert.AreEqual(0, DeletedRaised);
        Assert.AreEqual(0, RenamedRaised);
      }
      finally
      {
        subdir.Delete(true);
      }
    }
    #endregion

    #region NotifyFilter
    [TestMethod]
    public void RememberSettingForNotifyFilter()
    {
      FileSystemWatcher tested = new FileSystemWatcher();

      NotifyFilters filters = NotifyFilters.Attributes | NotifyFilters.Size | NotifyFilters.Security | NotifyFilters.LastAccess;
      tested.NotifyFilter = filters;
      Assert.AreEqual(filters, tested.NotifyFilter);
    }

    [TestMethod]
    public void EventsAreNotRasedIfNotMatchingNotifyFilter()
    {
      System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(".");
      string fileName = System.IO.Path.Combine(dir.FullName, Guid.NewGuid().ToString());
      string newFileName = System.IO.Path.Combine(dir.FullName, Guid.NewGuid().ToString());
      try
      {
        AutoResetEvent raised = new AutoResetEvent(false);
        FileSystemWatcher tested = new FileSystemWatcher(dir.FullName);
        tested.NotifyFilter = NotifyFilters.Security;
        tested.EnableRaisingEvents = true;
        int CreateRaised = 0;
        int ChangedRaised = 0;
        int DeletedRaised = 0;
        int RenamedRaised = 0;
        tested.Created += (s, e) => CreateRaised++;
        tested.Changed += (s, e) => ChangedRaised++;
        tested.Deleted += (s, e) => DeletedRaised++;
        tested.Renamed += (s, e) => RenamedRaised++;

        using (System.IO.FileStream file = System.IO.File.Create(fileName))
        {
          byte[] data = Encoding.UTF8.GetBytes("somedata");
          file.Write(data, 0, data.Length);
          file.Flush();
          file.Close();
        };
        Thread.Sleep(100);
        using (System.IO.FileStream file = System.IO.File.OpenWrite(fileName))
        {
          byte[] data = Encoding.UTF8.GetBytes("somedata");
          file.Write(data, 0, data.Length);
          file.Flush();
          file.Close();
        };
        Thread.Sleep(100);
        System.IO.File.Move(fileName, newFileName);
        Thread.Sleep(100);
        System.IO.File.Delete(newFileName);
        Thread.Sleep(100);

        Assert.AreEqual(0, CreateRaised);
        Assert.AreEqual(0, ChangedRaised);
        Assert.AreEqual(0, DeletedRaised);
        Assert.AreEqual(0, RenamedRaised);
      }
      finally
      {
        System.IO.File.Delete(fileName);
        System.IO.File.Delete(newFileName);
      }
    }
    #endregion

    #region Filter
    [TestMethod]
    public void RememberSettingForFilter()
    {
      FileSystemWatcher tested = new FileSystemWatcher();

      string filter = "*.fooBar";
      tested.Filter = filter;
      Assert.AreEqual(filter, tested.Filter);
    }

    [TestMethod]
    public void EventsAreNotRasedIfNotMatchingFilter()
    {
      System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(".");
      string fileName = System.IO.Path.Combine(dir.FullName, Guid.NewGuid().ToString());
      string newFileName = System.IO.Path.Combine(dir.FullName, Guid.NewGuid().ToString());
      try
      {
        AutoResetEvent raised = new AutoResetEvent(false);
        FileSystemWatcher tested = new FileSystemWatcher(dir.FullName);
        tested.Filter = "*.txt";
        tested.EnableRaisingEvents = true;
        int CreateRaised = 0;
        int ChangedRaised = 0;
        int DeletedRaised = 0;
        int RenamedRaised = 0;
        tested.Created += (s, e) => CreateRaised++;
        tested.Changed += (s, e) => ChangedRaised++;
        tested.Deleted += (s, e) => DeletedRaised++;
        tested.Renamed += (s, e) => RenamedRaised++;

        using (System.IO.FileStream file = System.IO.File.Create(fileName))
        {
          byte[] data = Encoding.UTF8.GetBytes("somedata");
          file.Write(data, 0, data.Length);
          file.Flush();
          file.Close();
        };
        Thread.Sleep(100);
        using (System.IO.FileStream file = System.IO.File.OpenWrite(fileName))
        {
          byte[] data = Encoding.UTF8.GetBytes("somedata");
          file.Write(data, 0, data.Length);
          file.Flush();
          file.Close();
        };
        Thread.Sleep(100);
        System.IO.File.Move(fileName, newFileName);
        Thread.Sleep(100);
        System.IO.File.Delete(newFileName);
        Thread.Sleep(100);

        Assert.AreEqual(0, CreateRaised);
        Assert.AreEqual(0, ChangedRaised);
        Assert.AreEqual(0, DeletedRaised);
        Assert.AreEqual(0, RenamedRaised);
      }
      finally
      {
        System.IO.File.Delete(fileName);
        System.IO.File.Delete(newFileName);
      }
    }

    [TestMethod]
    public void EventsShouldBeRasedIfMatchingFilter()
    {
      System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(".");
      string fileName = System.IO.Path.Combine(dir.FullName, Guid.NewGuid().ToString() + ".txt");
      string newFileName = System.IO.Path.Combine(dir.FullName, Guid.NewGuid().ToString() + ".txt");
      try
      {
        AutoResetEvent raised = new AutoResetEvent(false);
        FileSystemWatcher tested = new FileSystemWatcher(dir.FullName);
        tested.Filter = "*.txt";
        tested.EnableRaisingEvents = true;
        int CreateRaised = 0;
        int ChangedRaised = 0;
        int DeletedRaised = 0;
        int RenamedRaised = 0;
        tested.Created += (s, e) => CreateRaised++;
        tested.Changed += (s, e) => ChangedRaised++;
        tested.Deleted += (s, e) => DeletedRaised++;
        tested.Renamed += (s, e) => RenamedRaised++;

        using (System.IO.FileStream file = System.IO.File.Create(fileName))
        {
          byte[] data = Encoding.UTF8.GetBytes("somedata");
          file.Write(data, 0, data.Length);
          file.Flush();
          file.Close();
        };
        Thread.Sleep(100);
        using (System.IO.FileStream file = System.IO.File.OpenWrite(fileName))
        {
          byte[] data = Encoding.UTF8.GetBytes("somedata");
          file.Write(data, 0, data.Length);
          file.Flush();
          file.Close();
        };
        Thread.Sleep(100);
        System.IO.File.Move(fileName, newFileName);
        Thread.Sleep(100);
        System.IO.File.Delete(newFileName);
        Thread.Sleep(100);

        Assert.AreNotEqual(0, CreateRaised);
        Assert.AreNotEqual(0, ChangedRaised);
        Assert.AreNotEqual(0, DeletedRaised);
        Assert.AreNotEqual(0, RenamedRaised);
      }
      finally
      {
        System.IO.File.Delete(fileName);
        System.IO.File.Delete(newFileName);
      }
    }
    #endregion

    #region Disposing
    [TestMethod]
    public void ShouldRaiseDisposedOnDispose()
    {
      FileSystemWatcher tested = new FileSystemWatcher();
      bool wasDisposed = false;
      tested.Disposed += (s, e) => wasDisposed = true;
      tested.Dispose();
      Assert.IsTrue(wasDisposed);
    }

    [TestMethod]
    public void WatcherShouldBeDisposedOnDispose()
    {
      FileSystemWatcher tested = new FileSystemWatcher();
      bool wasDisposed = false;
      tested.watcher.Disposed += (s,e) => wasDisposed = true;
      tested.Dispose();
      Assert.IsTrue(wasDisposed);
    }
    #endregion

    #region Logging
    [TestMethod]
    public void DefaultConstructorShouldInitLog()
    {
      FileSystemWatcher tested = new FileSystemWatcher();
      ILogWriter logWriter = tested as ILogWriter;
      Assert.IsNotNull(logWriter.Log);
    }

    [TestMethod]
    public void ConstructingWithPathShouldInitLog()
    {
      System.IO.FileInfo thisAssemblyPath = new System.IO.FileInfo(GetType().Assembly.Location);
      FileSystemWatcher tested = new FileSystemWatcher(thisAssemblyPath.Directory.FullName);
      ILogWriter logWriter = tested as ILogWriter;
      Assert.IsNotNull(logWriter.Log);
    }

    [TestMethod]
    public void ConstructingWithPathAndFilterShouldInitLog()
    {
      System.IO.FileInfo thisAssemblyPath = new System.IO.FileInfo(GetType().Assembly.Location);
      FileSystemWatcher tested = new FileSystemWatcher(thisAssemblyPath.Directory.FullName, "filter");
      ILogWriter logWriter = tested as ILogWriter;
      Assert.IsNotNull(logWriter.Log);
    }

    [TestMethod]
    public void ShouldLogCreatedEventToDebug()
    {
      System.IO.FileInfo fileInfo = new System.IO.FileInfo(GetType().Assembly.Location);
      FileSystemWatcher tested = new FileSystemWatcher();
      MockLog log = new MockLog(tested);
      tested.OnCreated(tested, new System.IO.FileSystemEventArgs(System.IO.WatcherChangeTypes.Created, fileInfo.Directory.FullName, fileInfo.Name));
      Assert.IsTrue(log.Any(x => x.Level == MockLog.Level.Debug && x.Message.Contains(fileInfo.FullName)));
    }

    [TestMethod]
    public void ShouldLogChangedEventToDebug()
    {
      System.IO.FileInfo fileInfo = new System.IO.FileInfo(GetType().Assembly.Location);
      FileSystemWatcher tested = new FileSystemWatcher();
      MockLog log = new MockLog(tested);
      tested.OnChanged(tested, new System.IO.FileSystemEventArgs(System.IO.WatcherChangeTypes.Created, fileInfo.Directory.FullName, fileInfo.Name));
      Assert.IsTrue(log.Any(x => x.Level == MockLog.Level.Debug && x.Message.Contains(fileInfo.FullName)));
    }

    [TestMethod]
    public void ShouldLogDeletedEventToDebug()
    {
      System.IO.FileInfo fileInfo = new System.IO.FileInfo(GetType().Assembly.Location);
      FileSystemWatcher tested = new FileSystemWatcher();
      MockLog log = new MockLog(tested);
      tested.OnDeleted(tested, new System.IO.FileSystemEventArgs(System.IO.WatcherChangeTypes.Created, fileInfo.Directory.FullName, fileInfo.Name));
      Assert.IsTrue(log.Any(x => x.Level == MockLog.Level.Debug && x.Message.Contains(fileInfo.FullName)));
    }

    [TestMethod]
    public void ShouldLogRenamedEventToDebug()
    {
      System.IO.FileInfo fileInfo = new System.IO.FileInfo(GetType().Assembly.Location);
      FileSystemWatcher tested = new FileSystemWatcher();
      MockLog log = new MockLog(tested);
      tested.OnRenamed(tested, new System.IO.RenamedEventArgs(System.IO.WatcherChangeTypes.Created, fileInfo.Directory.FullName, "newname.dll", fileInfo.Name));
      Assert.IsTrue(log.Any(x => x.Level == MockLog.Level.Debug && x.Message.Contains(fileInfo.FullName)));
    }
    #endregion
  } 
}
