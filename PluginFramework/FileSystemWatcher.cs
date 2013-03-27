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
namespace PluginFramework
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Security.Permissions;
  using System.Text;
  using System.Threading;
  using System.Xml;
  using System.Xml.Linq;
  using System.ComponentModel;
  using PluginFramework.Logging;

  /// <summary>
  /// Thin wrapper to expose an <see cref="System.IO.FileSystemWatcher"/> as <see cref="IFileSystemWatcher"/>. Enabling mocking during unit testing.
  /// </summary>
  public sealed class FileSystemWatcher : IFileSystemWatcher, ILogWriter
  {
    internal System.IO.FileSystemWatcher watcher;

    private ILog log;
    ILog ILogWriter.Log
    {
      get { return this.log; }
      set { this.log = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemWatcher"/> class.
    /// </summary>
    [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
    public FileSystemWatcher()
    {
      this.InitLog();
      watcher = new System.IO.FileSystemWatcher();
      Connect();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemWatcher"/> class.
    /// </summary>
    /// <param name="path">The path to monitor</param>
    [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
    public FileSystemWatcher(string path)
      : this()
    {
      this.watcher.Path = path;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemWatcher"/> class.
    /// </summary>
    /// <param name="path">The path to monitor</param>
    /// <param name="filter">The filter to apply</param>
    [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
    public FileSystemWatcher(string path, string filter)
      : this(path)
    {
      this.watcher.Filter = filter;
    }

    /// <summary>
    /// Connects to the <see cref="System.IO.FileSystemWatcher"/>.
    /// </summary>
    [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
    private void Connect()
    {
      this.watcher.Changed += OnChanged;
      this.watcher.Created += OnCreated;
      this.watcher.Deleted += OnDeleted;
      this.watcher.Renamed += OnRenamed;
    }

    internal void OnCreated(object sender, System.IO.FileSystemEventArgs args)
    {
      this.log.Debug(Resources.OnCreated, args.FullPath);
      if (this.Created != null)
        this.Created(sender, new FileSystemEventArgs(args));
    }

    internal void OnChanged(object sender, System.IO.FileSystemEventArgs args)
    {
      this.log.Debug(Resources.OnChanged, args.FullPath);
      if (this.Changed != null)
        this.Changed(sender, new FileSystemEventArgs(args));
    }

    internal void OnDeleted(object s, System.IO.FileSystemEventArgs args)
    {
      this.log.Debug(Resources.OnDeleted, args.FullPath);
      if (this.Deleted != null)
        this.Deleted(s, new FileSystemEventArgs(args));
    }

    internal void OnRenamed(object sender, System.IO.RenamedEventArgs args)
    {
      this.log.Debug(Resources.OnRenamed, args.OldFullPath, args.Name);
      if (this.Renamed != null)
        this.Renamed(sender, new RenamedEventArgs(args));
    }

    /// <summary>
    /// Occurs when [changed].
    /// </summary>
    public event EventHandler<FileSystemEventArgs> Changed;

    /// <summary>
    /// Occurs when [created].
    /// </summary>
    public event EventHandler<FileSystemEventArgs> Created;

    /// <summary>
    /// Occurs when [deleted].
    /// </summary>
    public event EventHandler<FileSystemEventArgs> Deleted;

    /// <summary>
    /// Occurs when [renamed].
    /// </summary>
    public event EventHandler<RenamedEventArgs> Renamed;

    /// <summary>
    /// Gets or sets a value enabling of raising events.
    /// </summary>
    /// <value>
    ///   <c>true</c> if allowing events; otherwise, <c>false</c>.
    /// </value>
    public bool EnableRaisingEvents
    {
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      get
      {
        return watcher.EnableRaisingEvents;
      }
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      set
      {
        watcher.EnableRaisingEvents = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to also monitor subdirectories.
    /// </summary>
    /// <value>
    /// <c>true</c> if subdirectories should be monitored; otherwise, <c>false</c>.
    /// </value>
    public bool IncludeSubdirectories
    {
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      get
      {
        return watcher.IncludeSubdirectories;
      }
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      set
      {
        watcher.IncludeSubdirectories = value;
      }
    }

    /// <summary>
    /// Gets or sets the notify filter.
    /// </summary>
    /// <value>
    /// The notify filter.
    /// </value>
    public NotifyFilters NotifyFilter
    {
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      get
      {
        return (NotifyFilters)watcher.NotifyFilter;
      }
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      set
      {
        watcher.NotifyFilter = (System.IO.NotifyFilters)value;
      }
    }

    /// <summary>
    /// Gets or sets the filter.
    /// </summary>
    /// <value>
    /// The filter.
    /// </value>
    public string Filter
    {
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      get
      {
        return watcher.Filter;
      }
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      set
      {
        watcher.Filter = value;
      }
    }

    /// <summary>
    /// Gets or sets the path.
    /// </summary>
    /// <value>
    /// The path.
    /// </value>
    public string Path
    {
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      get
      {
        return watcher.Path;
      }
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      set
      {
        watcher.Path = value;
      }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);

      if (this.Disposed != null)
        this.Disposed(this, EventArgs.Empty);
    }

    /// <summary>
    /// Occurs when [disposed].
    /// </summary>
    public event EventHandler Disposed;

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    private void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (watcher != null)
        {
          watcher.Dispose();
          watcher = null;
        }
      }
    }
  }
}
