using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading;

namespace PluginFramework.Tests
{
  [TestClass]
  public class UnitTest_PluginDirectory
  {
    #region Construction
    [TestMethod]
    public void ConstructionRequiresPath()
    {
      DoAssert.Throws<ArgumentNullException>(() => new PluginDirectory(null, true));
    }

    [TestMethod]
    public void ConstructionRequiresValidDirectory()
    {
      DoAssert.Throws<ArgumentException>(() => new PluginDirectory(@"c:\" + Guid.NewGuid().ToString(), true));
    }

    [TestMethod]
    public void ConstructionWithValidPath()
    {
      System.IO.FileInfo fileInfo = new System.IO.FileInfo(GetType().Assembly.Location);
      IPluginDirectory tested = null;

      try
      {
        tested = new PluginDirectory(fileInfo.Directory.FullName, false);
      }
      catch { }

      Assert.IsNotNull(tested);
    }

    [TestMethod]
    public void ConstructionRequiresIFileSystemWatcher()
    {
      DoAssert.Throws<ArgumentNullException>(() => new PluginDirectory(null));
    }

    [TestMethod]
    public void ConstructionWithIFileSystemWatcher()
    {
      MockFileSystemWatcher mockwatcher = new MockFileSystemWatcher();
      IPluginDirectory tested = new PluginDirectory(mockwatcher);
    }
    #endregion

    #region FileFound
    [TestMethod]
    public void FileFoundShouldReportCurrentFilesFlatOnAdd()
    {
      MockFileSystemWatcher fsw = new MockFileSystemWatcher();
      fsw.Path = new FileInfo(GetType().Assembly.Location).Directory.FullName;

      PluginDirectory tested = new PluginDirectory(fsw);
      int FileFoundRaised = 0;
      tested.FileFound += (s, e) => FileFoundRaised++;

      Assert.AreNotEqual(0, FileFoundRaised);
    }

    [TestMethod]
    public void FileFoundShouldReportCurrentFilesRecursiveOnAdd()
    {
      MockFileSystemWatcher fsw = new MockFileSystemWatcher();
      fsw.Path = new FileInfo(GetType().Assembly.Location).Directory.Parent.FullName;
      fsw.IncludeSubdirectories = true;

      PluginDirectory tested = new PluginDirectory(fsw);
      int FileFoundRaised = 0;
      tested.FileFound += (s, e) => FileFoundRaised++;

      Assert.AreNotEqual(0, FileFoundRaised);
    }

    [TestMethod]
    public void FileFoundShouldBeRasedForDlls()
    {
      MockFileSystemWatcher fsw = new MockFileSystemWatcher();

      PluginDirectory tested = new PluginDirectory(fsw);
      int FileFoundRaised = 0;
      tested.FileFound += (s, e) => FileFoundRaised++;

      fsw.RaiseCreated(@"file.dll");

      Assert.AreEqual(1, FileFoundRaised);
    }

    [TestMethod]
    public void FileFoundShouldNotBeRasedForNonDlls()
    {
      MockFileSystemWatcher fsw = new MockFileSystemWatcher();

      PluginDirectory tested = new PluginDirectory(fsw);
      int FileFoundRaised = 0;
      tested.FileFound += (s, e) => FileFoundRaised++;

      fsw.RaiseCreated(@"file.img");

      Assert.AreEqual(0, FileFoundRaised);
    }

    [TestMethod]
    public void FileFoundShouldNotBeReportedAfterRemoval()
    {
      MockFileSystemWatcher fsw = new MockFileSystemWatcher();

      PluginDirectory tested = new PluginDirectory(fsw);
      int FileFoundRaised = 0;

      EventHandler<PluginDirectoryEventArgs> handler = ((s, e) => FileFoundRaised++);
      tested.FileFound += handler;
      tested.FileFound -= handler;

      fsw.RaiseCreated(@"file.dll");
      Assert.AreEqual(0, FileFoundRaised);
    }
    #endregion

    #region FileLost
    [TestMethod]
    public void FileLostShouldBeRasedForDlls()
    {
      MockFileSystemWatcher fsw = new MockFileSystemWatcher();

      PluginDirectory tested = new PluginDirectory(fsw);
      int FileLostRaised = 0;
      tested.FileLost += (s, e) => FileLostRaised++;

      fsw.RaiseDeleted(@"file.dll");

      Assert.AreEqual(1, FileLostRaised);
    }

    [TestMethod]
    public void FileLostShouldNotBeRasedForNonDlls()
    {
      MockFileSystemWatcher fsw = new MockFileSystemWatcher();

      PluginDirectory tested = new PluginDirectory(fsw);
      int FileLostRaised = 0;
      tested.FileLost += (s, e) => FileLostRaised++;

      fsw.RaiseDeleted(@"file.img");

      Assert.AreEqual(0, FileLostRaised);
    }
    #endregion

    #region IFileSystemWatcher
    [TestMethod]
    public void CreatedShouldRaiseFileFound()
    {
      MockFileSystemWatcher fsw = new MockFileSystemWatcher();
      PluginDirectory tested = new PluginDirectory(fsw);
      int FileFoundRaised = 0;
      tested.FileFound += (s, e) => FileFoundRaised++;
      fsw.RaiseCreated(GetType().Assembly.Location);
      Assert.AreEqual(1, FileFoundRaised);
    }

    [TestMethod]
    public void ChangedShouldRaiseFileLostFollowedByFileFound()
    {
      MockFileSystemWatcher fsw = new MockFileSystemWatcher();
      PluginDirectory tested = new PluginDirectory(fsw);
      DateTime FileFoundRaised = DateTime.MinValue;
      DateTime FileLostRaised = DateTime.MinValue;
      tested.FileFound += (s, e) => { FileFoundRaised = DateTime.Now; Thread.Sleep(1); };
      tested.FileLost += (s, e) => { FileLostRaised = DateTime.Now; Thread.Sleep(1); };
      fsw.RaiseChanged(GetType().Assembly.Location);
      Assert.AreNotEqual(DateTime.MinValue, FileFoundRaised);
      Assert.AreNotEqual(DateTime.MinValue, FileLostRaised);
      Assert.IsTrue(FileLostRaised < FileFoundRaised);
    }

    [TestMethod]
    public void DeletedShouldRaiseFileLost()
    {
      MockFileSystemWatcher fsw = new MockFileSystemWatcher();
      PluginDirectory tested = new PluginDirectory(fsw);
      int FileLostRaised = 0;
      tested.FileLost += (s, e) => FileLostRaised++;
      fsw.RaiseDeleted(GetType().Assembly.Location);
      Assert.AreEqual(1, FileLostRaised);
    }

    [TestMethod]
    public void RenamedShouldRaiseFileLostFollowedByFileFound()
    {
      MockFileSystemWatcher fsw = new MockFileSystemWatcher();
      PluginDirectory tested = new PluginDirectory(fsw);
      DateTime FileFoundRaised = DateTime.MinValue;
      DateTime FileLostRaised = DateTime.MinValue;
      tested.FileFound += (s, e) => { FileFoundRaised = DateTime.Now; Thread.Sleep(1); };
      tested.FileLost += (s, e) => { FileLostRaised = DateTime.Now; Thread.Sleep(1); };
      fsw.RaiseRenamed(GetType().Assembly.Location, GetType().Assembly.Location);
      Assert.AreNotEqual(DateTime.MinValue, FileFoundRaised);
      Assert.AreNotEqual(DateTime.MinValue, FileLostRaised);
      Assert.IsTrue(FileLostRaised < FileFoundRaised);
    }
    #endregion

    #region IDisposable
    [TestMethod]
    public void ShouldDisposeOwnedFileSystemWatcher()
    {
      MockFileSystemWatcher mockwatcher = new MockFileSystemWatcher();
      PluginDirectory tested = new PluginDirectory(new MockFileSystemWatcher());
      bool wasDisposed = false;
      tested.WatcherConnect(mockwatcher, true);
      mockwatcher.Disposed += (s, e) => wasDisposed = true;
      tested.Dispose();
      Assert.IsTrue(wasDisposed);
    }

    [TestMethod]
    public void ShouldNotDisposeExternalIFileSystemWatcher()
    {
      MockFileSystemWatcher mockwatcher = new MockFileSystemWatcher();
      IPluginDirectory tested = new PluginDirectory(mockwatcher);
      bool wasDisposed = false;
      mockwatcher.Disposed += (s, e) => wasDisposed = true;
      tested.Dispose();
      Assert.IsFalse(wasDisposed);
    }
    #endregion
  }
}
