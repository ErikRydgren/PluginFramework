using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Permissions;

namespace PluginFramework.Tests.Mocks
{
  public class MockFileSystemWatcher : IFileSystemWatcher
  {
    public event EventHandler<FileSystemEventArgs> Created;
    public event EventHandler<FileSystemEventArgs> Changed;
    public event EventHandler<FileSystemEventArgs> Deleted;
    public event EventHandler<RenamedEventArgs> Renamed;
    public event EventHandler Disposed;

    public Delegate[] CreatedListeners()
    {
      return Created.GetInvocationList();
    }

    public Delegate[] ChangedListeners()
    {
      return Changed.GetInvocationList();
    }

    public Delegate[] DeletedListeners()
    {
      return Deleted.GetInvocationList();
    }

    public Delegate[] RenamedListeners()
    {
      return Renamed.GetInvocationList();
    }

    public void RaiseCreated(string path)
    {
      if (this.Created != null)
      {
        FileInfo fileInfo = new FileInfo(path);
        var args = new FileSystemEventArgs(WatcherChangeTypes.Created, fileInfo.Directory.FullName, fileInfo.Name);
        this.Created(this, args);
      }
    }

    public void RaiseChanged(string path)
    {
      if (this.Changed != null)
      {
        FileInfo fileInfo = new FileInfo(path);
        var args = new FileSystemEventArgs(WatcherChangeTypes.Changed, fileInfo.Directory.FullName, fileInfo.Name);
        this.Changed(this, args);
      }
    }

    public void RaiseDeleted(string path)
    {
      if (this.Deleted != null)
      {
        FileInfo fileInfo = new FileInfo(path);
        var args = new FileSystemEventArgs(WatcherChangeTypes.Deleted, fileInfo.Directory.FullName, fileInfo.Name);
        this.Deleted(this, args);
      }
    }

    public void RaiseRenamed(string from, string to)
    {
      if (this.Renamed != null)
      {
        FileInfo fromFile = new FileInfo(from);
        FileInfo toFile = new FileInfo(to);
        var args = new RenamedEventArgs(WatcherChangeTypes.Renamed, toFile.Directory.FullName, toFile.Name, fromFile.Name);
        this.Renamed(this, args);
      }
    }

    public void RaiseDisposed()
    {
      if (this.Disposed != null)
        this.Disposed(this, EventArgs.Empty);
    }

    public bool EnableRaisingEvents
    {
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      get;
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      set;
    }

    public bool IncludeSubdirectories
    {
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      get;
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      set;
    }

    public NotifyFilters NotifyFilter
    {
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      get;
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      set;
    }

    public string Filter
    {
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      get;
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      set;
    }

    public string Path
    {
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      get;
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      set;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    public void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.Disposed != null)
          this.Disposed(this, EventArgs.Empty);
      }
    }
  }
}
