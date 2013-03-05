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
  using System.Diagnostics;
  using System.IO;
  using System.Linq;

  /// <summary>
  /// Holds and publishes registered assemblies. Also provides functionality for loading from directorytree and react to changes in the tree.
  /// Implements and exposes <seealso cref="IAssemblySource"/> and <seealso cref="IAssemblyRepository"/>.
  /// </summary>
  public class AssemblyContainer : MarshalByRefObject, IAssemblySource, IAssemblyRepository
  {
    Dictionary<string, List<string>> assemblyPaths = new Dictionary<string, List<string>>();
    Dictionary<string, string> pathAssembly = new Dictionary<string, string>();
    Dictionary<DirectoryInfo, FileSystemWatcher> watchedDirs = new Dictionary<DirectoryInfo, FileSystemWatcher>();

    public event EventHandler<AssemblyAddedEventArgs> AssemblyAdded;
    public event EventHandler<AssemblyRemovedEventArgs> AssemblyRemoved;

    public bool Add(FileInfo assemblyFile)
    {
      if (!assemblyFile.Exists)
        throw new FileNotFoundException();

      using (AssemblyReflectionManager reflectionManager = new AssemblyReflectionManager())
      {
        try
        {
          lock (this.assemblyPaths)
          {
            if (!reflectionManager.LoadAssembly(assemblyFile.FullName, Guid.NewGuid().ToString()))
              return false;

            var assemblyName = reflectionManager.Reflect(assemblyFile.FullName, assembly => assembly.FullName);
            
            List<string> paths;
            if (this.assemblyPaths.TryGetValue(assemblyName, out paths) == false)
            {
              paths = new List<string>();
              this.assemblyPaths.Add(assemblyName, paths);
            }
            paths.Add(assemblyFile.FullName);
            pathAssembly.Add(assemblyFile.FullName, assemblyName);

            if (this.AssemblyAdded != null)
              this.AssemblyAdded(this, new AssemblyAddedEventArgs(assemblyFile.FullName, reflectionManager));

            Debug.WriteLine("AssemblyContainer added {0}", (object)assemblyFile.FullName);
          }
          return true;
        }
        catch (Exception)
        {
          return false;
        }
      }
    }

    public void Remove(FileInfo assemblyFile)
    {
      lock (this.assemblyPaths)
      {
        string assemblyName;
        if (this.pathAssembly.TryGetValue(assemblyFile.FullName, out assemblyName) == false)
          return;

        this.pathAssembly.Remove(assemblyFile.FullName);

        List<string> paths;
        if (this.assemblyPaths.TryGetValue(assemblyName, out paths) == false)
          return;

        paths.Remove(assemblyFile.FullName);
        if (paths.Count == 0)
          this.assemblyPaths.Remove(assemblyName);

        if (this.AssemblyRemoved != null)
          this.AssemblyRemoved(this, new AssemblyRemovedEventArgs(assemblyFile.FullName));

        Debug.WriteLine("AssemblyContainer removed {0}", (object)assemblyFile.FullName);
      }
    }

    public void SyncWithDirectory(DirectoryInfo dir, bool includeSubdirectories)
    {
      if (!dir.Exists)
        throw new ArgumentException("Directory does not exist");

      FileSystemWatcher watcher = new FileSystemWatcher(dir.FullName, "*.dll");
      watcher.IncludeSubdirectories = includeSubdirectories;
      watcher.NotifyFilter = NotifyFilters.FileName;
      watcher.Created += new FileSystemEventHandler(ModuleAdded);
      watcher.Changed += new FileSystemEventHandler(ModuleChanged);
      watcher.Deleted += new FileSystemEventHandler(ModuleRemoved);
      watcher.Renamed += new RenamedEventHandler(ModuleRenamed);

      foreach (var file in dir.GetFiles("*.dll", SearchOption.AllDirectories))
        this.Add(file);

      watcher.EnableRaisingEvents = true;

      watchedDirs.Add(dir, watcher);
    }

    public void UnsyncWithDirectory(DirectoryInfo dir)
    {
      var pair = watchedDirs.FirstOrDefault(x => x.Key.FullName == dir.FullName);
      if (pair.Key == null)
        return;

      pair.Value.EnableRaisingEvents = false;
      pair.Value.Dispose();
      watchedDirs.Remove(pair.Key);
    }

    void ModuleAdded(object sender, FileSystemEventArgs e)
    {
      this.Add(new FileInfo(e.FullPath));
    }

    void ModuleChanged(object sender, FileSystemEventArgs e)
    {
      FileInfo fileInfo = new FileInfo(e.FullPath);
      this.Remove(fileInfo);
      this.Add(fileInfo);
    }

    void ModuleRemoved(object sender, FileSystemEventArgs e)
    {
      this.Remove(new FileInfo(e.FullPath));
    }

    void ModuleRenamed(object sender, RenamedEventArgs e)
    {
      this.Remove(new FileInfo(e.OldFullPath));
      this.Add(new FileInfo(e.FullPath));
    }

    byte[] IAssemblyRepository.Get(string assemblyFullName)
    {
      byte[] buffer = null;
      FileInfo assemblyFile = null;

      lock (this.assemblyPaths)
      {
        List<string> paths;
        if (!this.assemblyPaths.TryGetValue(assemblyFullName, out paths))
          return null;

        assemblyFile = paths.Select(path => new FileInfo(path)).FirstOrDefault(file => file.Exists);
      }

      if (assemblyFile != null)
      {
        try
        {
          buffer = new byte[assemblyFile.Length];
          using (var fileStream = assemblyFile.OpenRead())
          {
            if (fileStream.Read(buffer, 0, (int)assemblyFile.Length) != assemblyFile.Length)
              buffer = null;
          }
        }
        catch (IOException)
        {
          buffer = null;
        }
      }

      return buffer;
    }
  }
}
