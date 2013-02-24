using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace PluginFramework
{
  /// <summary>
  /// Holds and publishes registered assemblies. Also provides functionality for loading from directorytree and react to changes in the tree.
  /// Implements and exposes <seealso cref="IAssemblySource"/> and <seealso cref="IAssemblyRepository"/>.
  /// </summary>
  public class AssemblyContainer : MarshalByRefObject, IAssemblySource, IAssemblyRepository
  {
    Dictionary<string, FileInfo> assemblyFiles = new Dictionary<string, FileInfo>();
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
          lock (this.assemblyFiles)
          {
            reflectionManager.LoadAssembly(assemblyFile.FullName, Guid.NewGuid().ToString());
            var assemblyName = reflectionManager.Reflect(assemblyFile.FullName, assembly => assembly.FullName);
            this.assemblyFiles.Add(assemblyName, assemblyFile);

            if (this.AssemblyAdded != null)
              this.AssemblyAdded(this, new AssemblyAddedEventArgs(assemblyFile, reflectionManager));

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
      lock (this.assemblyFiles)
      {
        var pair = assemblyFiles.FirstOrDefault(x => x.Value.FullName == assemblyFile.FullName);
        if (pair.Key == null)
          return;

        assemblyFiles.Remove(pair.Key);
        if (this.AssemblyRemoved != null)
          this.AssemblyRemoved(this, new AssemblyRemovedEventArgs(pair.Key));

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
      FileInfo fileInfo = null;
      lock (this.assemblyFiles)
      {
        if (!this.assemblyFiles.TryGetValue(assemblyFullName, out fileInfo))
          return null;
      }

      fileInfo.Refresh();

      if (fileInfo.Exists == false)
        return null;

      byte[] buffer = new byte[fileInfo.Length];
      using (var fileStream = fileInfo.OpenRead())
      {
        if (fileStream.Read(buffer, 0, (int)fileInfo.Length) != fileInfo.Length)
          return null;
      }

      return buffer;
    }
  }
}
