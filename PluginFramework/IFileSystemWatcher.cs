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
  using System.ComponentModel;
  using System.Linq;
  using System.Security.Permissions;
  using System.Text;

  /// <summary>
  /// Wrapper for <see cref="System.IO.FileSystemEventArgs"/>
  /// </summary>
  public class FileSystemEventArgs : EventArgs
  {
    internal FileSystemEventArgs(System.IO.FileSystemEventArgs args)
    {      
      this.ChangeType = (WatcherChangeTypes)args.ChangeType;
      this.FullPath = args.FullPath;
      this.Name = args.Name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemEventArgs" /> class.
    /// </summary>
    /// <param name="changeType">Type of the change.</param>
    /// <param name="directory">The directory.</param>
    /// <param name="name">The name.</param>
    public FileSystemEventArgs(WatcherChangeTypes changeType, string directory, string name)
    {
      this.ChangeType = changeType;
      this.FullPath = System.IO.Path.Combine(directory, name);
      this.Name = name;
    }

    /// <summary>
    /// Gets the type of the change.
    /// </summary>
    /// <value>
    /// The type of the change.
    /// </value>
    public WatcherChangeTypes ChangeType { get; private set; }

    /// <summary>
    /// Gets the full path.
    /// </summary>
    /// <value>
    /// The full path.
    /// </value>
    public string FullPath { get; private set; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; private set; }
  }

  /// <summary>
  /// Wrapper for <see cref="System.IO.RenamedEventArgs"/>
  /// </summary>
  public class RenamedEventArgs : FileSystemEventArgs
  {
    internal RenamedEventArgs(System.IO.RenamedEventArgs args)
      : base(args)
    {
      this.OldFullPath = args.OldFullPath;
      this.OldName = args.OldName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RenamedEventArgs" /> class.
    /// </summary>
    /// <param name="changeType">Type of the change.</param>
    /// <param name="directory">The directory.</param>
    /// <param name="name">The name.</param>
    /// <param name="oldName">The old name.</param>
    public RenamedEventArgs(WatcherChangeTypes changeType, string directory, string name, string oldName)
      : base(changeType, directory, name)
    {
      this.OldFullPath = System.IO.Path.Combine(directory, oldName);
      this.OldName = oldName;
    }

    /// <summary>
    /// Gets the old full path.
    /// </summary>
    /// <value>
    /// The old full path.
    /// </value>
    public string OldFullPath { get; private set; }

    /// <summary>
    /// Gets the old name.
    /// </summary>
    /// <value>
    /// The old name.
    /// </value>
    public string OldName { get; private set; }
  }

  /// <summary>
  /// Changes that might occur to a file or directory.
  /// </summary>
  [Flags]
  public enum WatcherChangeTypes
  {
    /// <summary>
    /// The creation of a file or folder.
    /// </summary>
    Created = 1,
    
    /// <summary>
    /// The deletion of a file or folder.
    /// </summary>
    Deleted = 2,
    
    /// <summary>
    /// The change of a file or folder. The types of changes include: changes to size, attributes, security settings, last write, and last access time.
    /// </summary>
    Changed = 4,
    
    /// <summary>
    /// The renaming of a file or folder.
    /// </summary>
    Renamed = 8,
    
    /// <summary>
    /// The creation, deletion, change, or renaming of a file or folder.
    /// </summary>
    All = 15,
  }

  /// <summary>
  /// Specifies changes to watch for in a file or folder.
  /// </summary>
  [Flags]
  public enum NotifyFilters
  {
    /// <summary>
    /// The name of the file.
    /// </summary>
    FileName = 1,
    
    /// <summary>
    /// The name of the directory.
    /// </summary>
    DirectoryName = 2,
    
    /// <summary>
    /// The attributes of the file or folder.
    /// </summary>
    Attributes = 4,
    
    /// <summary>
    /// The size of the file or folder.
    /// </summary>
    Size = 8,
    
    /// <summary>
    /// The date the file or folder last had anything written to it.
    /// </summary>     
    LastWrite = 16,
    
    /// <summary>
    /// The date the file or folder was last opened.
    /// </summary>     
    LastAccess = 32,
    
    /// <summary>
    /// The time the file or folder was created.
    /// </summary>
    CreationTime = 64,

    /// <summary>
    /// The security settings of the file or folder.
    /// </summary>     
    Security = 256,
  }

  /// <summary>
  /// Interface abstraction for an <see cref="System.IO.FileSystemWatcher"/>. 
  /// </summary>
  public interface IFileSystemWatcher : IDisposable
  {
    /// <summary>
    /// Occurs when [changed].
    /// </summary>
    event EventHandler<FileSystemEventArgs> Changed;

    /// <summary>
    /// Occurs when [created].
    /// </summary>
    event EventHandler<FileSystemEventArgs> Created;

    /// <summary>
    /// Occurs when [deleted].
    /// </summary>
    event EventHandler<FileSystemEventArgs> Deleted;

    /// <summary>
    /// Occurs when [renamed].
    /// </summary>
    event EventHandler<RenamedEventArgs> Renamed;

    /// <summary>
    /// Occurs when [disposed].
    /// </summary>
    event EventHandler Disposed;

    /// <summary>
    /// Gets or sets a value indicating whether [enable raising events].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable raising events]; otherwise, <c>false</c>.
    /// </value>
    bool EnableRaisingEvents {
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      get;
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      set; 
    }

    /// <summary>
    /// Gets or sets a value indicating whether [include subdirectories].
    /// </summary>
    /// <value>
    /// <c>true</c> if [include subdirectories]; otherwise, <c>false</c>.
    /// </value>
    bool IncludeSubdirectories {
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      get;
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      set; 
    }

    /// <summary>
    /// Gets or sets the notify filter.
    /// </summary>
    /// <value>
    /// The notify filter.
    /// </value>
    NotifyFilters NotifyFilter {
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      get;
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      set; 
    }

    /// <summary>
    /// Gets or sets the filter.
    /// </summary>
    /// <value>
    /// The filter.
    /// </value>
    string Filter {
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      get;
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      set; 
    }

    /// <summary>
    /// Gets or sets the path.
    /// </summary>
    /// <value>
    /// The path.
    /// </value>
    string Path {
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      get;
      [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
      set; 
    }
  }
}
