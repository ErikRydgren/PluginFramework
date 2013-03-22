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
  using System.Text;
  using System.IO;
  using System.Threading;
  using System.Xml;
  using System.Xml.Linq;
  using System.Security.Permissions;

  /// <summary>
  /// A <see cref="IFileSystemWatcher"/> that delays events until the event file is readable.
  /// It also aggregates events so only relevant events get raised.
  /// </summary>
  public sealed class SafeEventFileSystemWatcher : IFileSystemWatcher
  {
    internal enum WatcherEventType
    {
      Created,
      Changed,
      Deleted,
      Renamed,
    };

    internal class WatcherEvent
    {
      public WatcherEvent(WatcherEventType eventType, FileSystemEventArgs args)
      {
        this.Event = eventType;
        this.Args = args;
      }

      public WatcherEventType Event { get; internal set; }
      public FileSystemEventArgs Args { get; internal set; }
    }

    IFileSystemWatcher watcher = null;
    Dictionary<string, List<WatcherEvent>> fileEvents = new Dictionary<string, List<WatcherEvent>>();
    AutoResetEvent gotEvent = new AutoResetEvent(false);
    Thread eventProcessor = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeEventFileSystemWatcher"/> class.
    /// </summary>
    /// <param name="watcher">The watcher.</param>
    public SafeEventFileSystemWatcher(IFileSystemWatcher watcher)
    {
      if (watcher == null)
        throw new ArgumentNullException("watcher");

      this.watcher = watcher;
      this.Initialize();
    }

    /// <summary>
    /// Gets or sets a value indicating whether [enable raising events].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable raising events]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableRaisingEvents
    {
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      get { return this.watcher.EnableRaisingEvents; }
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      set { this.watcher.EnableRaisingEvents = value; }
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
      get { return this.watcher.Filter; }
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      set { this.watcher.Filter = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [include subdirectories].
    /// </summary>
    /// <value>
    /// <c>true</c> if [include subdirectories]; otherwise, <c>false</c>.
    /// </value>
    public bool IncludeSubdirectories
    {
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      get { return this.watcher.IncludeSubdirectories; }
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      set { this.watcher.IncludeSubdirectories = value; }
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
      get { return this.watcher.NotifyFilter; }
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      set { this.watcher.NotifyFilter = value; }
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
      get { return this.watcher.Path; }
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      set { this.watcher.Path = value; }
    }

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    private void Initialize()
    {
      this.eventProcessor = new Thread(ProcessEvents);
      this.eventProcessor.Start();

      watcher.Created += watcher_Created;
      watcher.Changed += watcher_Changed;
      watcher.Deleted += watcher_Deleted;
      watcher.Renamed += watcher_Renamed;
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
    /// Entry point for background event processor.
    /// </summary>
    private void ProcessEvents()
    {
      while (true)
      {
        gotEvent.WaitOne(250, false);

        List<WatcherEvent>[] eventsToProcess = GetEventsToProcess();

        foreach (List<WatcherEvent> events in eventsToProcess)
        {
          Compact(events);
          foreach (var evt in events)
            RaiseEvent(evt);
        }
      }
    }

    internal void RaiseEvent(WatcherEvent fileEvent)
    {
      if (fileEvent == null)
        throw new ArgumentNullException("fileEvent");

      switch (fileEvent.Event)
      {
        case WatcherEventType.Created:
          {
            if (this.Created != null)
              this.Created(this, fileEvent.Args);
          }
          break;

        case WatcherEventType.Changed:
          {
            if (this.Changed != null)
              this.Changed(this, fileEvent.Args);
          }
          break;

        case WatcherEventType.Deleted:
          {
            if (this.Deleted != null)
              this.Deleted(this, fileEvent.Args);
          }
          break;

        case WatcherEventType.Renamed:
          {
            if (this.Renamed != null)
              this.Renamed(this, fileEvent.Args as RenamedEventArgs);
          }
          break;

        default:
          throw new ArgumentException("Unknown WatcherEventType " + fileEvent.Event.ToString());
      }
    }

    private List<WatcherEvent>[] GetEventsToProcess()
    {
      lock (fileEvents)
      {
        var filesToProcess = fileEvents.Where(x =>
        {
          if (!File.Exists(x.Key))
            if (x.Value.Any(e => e.Event == WatcherEventType.Deleted))
              return true;
            else
              return false;

          return IsReadable(x.Key);
        }).ToArray();

        foreach (var item in filesToProcess)
          fileEvents.Remove(item.Key);

        return filesToProcess.Select(x => x.Value).ToArray();
      }
    }

    private static void Compact(List<WatcherEvent> events)
    {
      bool gotCreated = events.Any(x => x.Event == WatcherEventType.Created);
      bool gotDeleted = events.Any(x => x.Event == WatcherEventType.Deleted);

      if (gotCreated && gotDeleted)
      {
        events.Clear();
        return;
      }

      if (gotCreated)
      {
        events.RemoveAll(x => x.Event == WatcherEventType.Changed);
        return;
      }

      var redundantChanged = events.Where(x => x.Event == WatcherEventType.Changed).Where((x, i) => i > 0).ToArray();
      foreach (var evt in redundantChanged)
        events.Remove(evt);
    }

    /// <summary>
    /// Handles the Created event of the watcher control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
    private void watcher_Created(object sender, FileSystemEventArgs e)
    {
      lock (this.fileEvents)
      {
        List<WatcherEvent> events;
        if (!fileEvents.TryGetValue(e.FullPath, out events))
          fileEvents.Add(e.FullPath, events = new List<WatcherEvent>());

        events.Add(new WatcherEvent(WatcherEventType.Created, e));
        this.gotEvent.Set();
      }
    }

    /// <summary>
    /// Handles the Changed event of the watcher control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
    private void watcher_Changed(object sender, FileSystemEventArgs e)
    {
      lock (this.fileEvents)
      {
        List<WatcherEvent> events;
        if (!fileEvents.TryGetValue(e.FullPath, out events))
          fileEvents.Add(e.FullPath, events = new List<WatcherEvent>());

        events.Add(new WatcherEvent(WatcherEventType.Changed, e));
        this.gotEvent.Set();
      }
    }

    /// <summary>
    /// Handles the Deleted event of the watcher control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
    private void watcher_Deleted(object sender, FileSystemEventArgs e)
    {
      lock (this.fileEvents)
      {
        List<WatcherEvent> events;
        if (!fileEvents.TryGetValue(e.FullPath, out events))
          fileEvents.Add(e.FullPath, events = new List<WatcherEvent>());

        events.Add(new WatcherEvent(WatcherEventType.Deleted, e));
        this.gotEvent.Set();
      }
    }

    /// <summary>
    /// Handles the Renamed event of the watcher control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="RenamedEventArgs"/> instance containing the event data.</param>
    private void watcher_Renamed(object sender, RenamedEventArgs e)
    {
      lock (this.fileEvents)
      {
        List<WatcherEvent> events;
        if (!fileEvents.TryGetValue(e.FullPath, out events))
          fileEvents.Add(e.FullPath, events = new List<WatcherEvent>());

        events.Add(new WatcherEvent(WatcherEventType.Renamed, e));
        this.gotEvent.Set();
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
    public void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.watcher != null)
        {
          this.watcher.Dispose();
          this.watcher = null;
        }

        if (this.eventProcessor != null)
        {
          this.eventProcessor.Abort();
          this.eventProcessor.Join();
          this.eventProcessor = null;
        }

        if (this.gotEvent != null)
        {
          this.gotEvent.Close();
          this.gotEvent = null;
        }
      }
    }

    private static bool IsReadable(string filePath)
    {
      try
      {
        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
          return true;
      }
      catch (IOException)
      {
        return false;
      }
    }
  }
}
