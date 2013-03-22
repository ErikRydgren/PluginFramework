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
  using System.IO;
  using System.Linq;
  using System.Security.Permissions;
  using System.Text;

  /// <summary>
  /// Monitors changes to a directory and reports modifications to dll's within the directory as <see cref="IPluginDirectory"/> events.
  /// </summary>
  public sealed class PluginDirectory : IPluginDirectory
  {
    IFileSystemWatcher watcher;
    bool ownsWatcher;
    private event EventHandler<PluginDirectoryEventArgs> fileFound;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginDirectory"/> class.
    /// </summary>
    /// <param name="dir">The directory to watch</param>
    /// <param name="includeSubdirectories">if set to <c>true</c> then subdirectories are also watched.</param>
    /// <exception cref="System.ArgumentNullException">dir</exception>
    /// <exception cref="System.ArgumentException">Directory does not exist</exception>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    public PluginDirectory(string dir, bool includeSubdirectories)
    {
      if (dir == null)
        throw new ArgumentNullException("dir");

      DirectoryInfo directory = new DirectoryInfo(dir);
      if (!directory.Exists)
        throw new ArgumentException("Directory does not exist");

      IFileSystemWatcher fsw = new SafeEventFileSystemWatcher(new FileSystemWatcher(directory.FullName, "*.dll"));
      fsw.IncludeSubdirectories = includeSubdirectories;
      WatcherConnect(fsw, true);
      this.watcher.EnableRaisingEvents = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginDirectory"/> class.
    /// </summary>
    /// <param name="watcher">The <see cref="IFileSystemWatcher"/> that rases events about file changes.</param>
    /// <exception cref="System.ArgumentNullException">watcher</exception>
    public PluginDirectory(IFileSystemWatcher watcher)
    {
      if (watcher == null)
        throw new ArgumentNullException("watcher");

      WatcherConnect(watcher, false);
    }

    internal void WatcherConnect(IFileSystemWatcher fileSystemWatcher, bool adopt)
    {
      if (fileSystemWatcher != null)
      {
        this.watcher = fileSystemWatcher;
        this.ownsWatcher = adopt;
        this.watcher.Created += watcher_Created;
        this.watcher.Changed += watcher_Changed;
        this.watcher.Deleted += watcher_Deleted;
        this.watcher.Renamed += watcher_Renamed;
      }
    }

    void watcher_Created(object sender, FileSystemEventArgs e)
    {
      this.OnFileFound(new PluginDirectoryEventArgs(e.FullPath));
    }

    void watcher_Changed(object sender, FileSystemEventArgs e)
    {
      this.OnFileLost(new PluginDirectoryEventArgs(e.FullPath));
      this.OnFileFound(new PluginDirectoryEventArgs(e.FullPath));
    }

    void watcher_Deleted(object sender, FileSystemEventArgs e)
    {
      this.OnFileLost(new PluginDirectoryEventArgs(e.FullPath));
    }

    void watcher_Renamed(object sender, RenamedEventArgs e)
    {
      this.OnFileLost(new PluginDirectoryEventArgs(e.OldFullPath));
      this.OnFileFound(new PluginDirectoryEventArgs(e.FullPath));
    }

    /// <summary>
    /// Raises the <see cref="E:FileFound" /> event.
    /// </summary>
    /// <param name="args">The <see cref="PluginDirectoryEventArgs"/> instance containing the event data.</param>
    private void OnFileFound(PluginDirectoryEventArgs args)
    {
      if (this.fileFound != null)
      {
        FileInfo file = new FileInfo(args.FullName);
        if (string.Compare(file.Extension, ".dll", StringComparison.OrdinalIgnoreCase) == 0)
          this.fileFound(this, args);
      }
    }

    /// <summary>
    /// Raises the <see cref="E:FileLost" /> event.
    /// </summary>
    /// <param name="args">The <see cref="PluginDirectoryEventArgs"/> instance containing the event data.</param>
    private void OnFileLost(PluginDirectoryEventArgs args)
    {
      if (this.FileLost != null)
      {
        FileInfo file = new FileInfo(args.FullName);
        if (string.Compare(file.Extension, ".dll", StringComparison.OrdinalIgnoreCase) == 0)
          this.FileLost(this, args);
      }
    }

    /// <summary>
    /// Occurs when a assembly file is found.
    /// </summary>
    public event EventHandler<PluginDirectoryEventArgs> FileFound
    {
      [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
      add
      {
        this.fileFound += value;

        if (value != null)
        {
          if (this.watcher.Path == null)
            return;

          DirectoryInfo directory = new DirectoryInfo(this.watcher.Path);
          if (directory.Exists)
            foreach (var file in directory.GetFiles("*.dll", this.watcher.IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
              value(this, new PluginDirectoryEventArgs(file.FullName));
        }
      }

      remove
      {
        this.fileFound -= value;
      }
    }

    /// <summary>
    /// Occurs when a assembly file is lost.
    /// </summary>
    public event EventHandler<PluginDirectoryEventArgs> FileLost;

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    private void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.ownsWatcher && this.watcher != null)
          this.watcher.Dispose();
        this.watcher = null;
      }
    }
  }
}
