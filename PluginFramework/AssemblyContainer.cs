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
  using System.Security.Permissions;
  using System.Security;
  using System.Threading;

  /// <summary>
  /// Holds and publishes registered assemblies. Also provides functionality for loading from directorytree and react to changes in the tree.
  /// Implements and exposes <see cref="IAssemblySource" /> and <see cref="IAssemblyRepository" />.
  /// </summary>
  public sealed class AssemblyContainer : MarshalByRefObject, IAssemblySource, IAssemblyRepository
  {
    Dictionary<string, List<string>> assemblyPaths = new Dictionary<string, List<string>>();
    Dictionary<string, string> pathAssembly = new Dictionary<string, string>();
    HashSet<IPluginDirectory> directories = new HashSet<IPluginDirectory>();

    /// <summary>
    /// Occurs when an assembly is added.
    /// </summary>
    public event EventHandler<AssemblyAddedEventArgs> AssemblyAdded;

    /// <summary>
    /// Occurs when an assembly removed.
    /// </summary>
    public event EventHandler<AssemblyRemovedEventArgs> AssemblyRemoved;

    /// <summary>
    /// Adds the specified assembly file.
    /// </summary>
    /// <param name="assemblyPath">The assembly path.</param>
    /// <returns>
    ///   <c>true</c> if the assembly was added
    /// </returns>
    /// <exception cref="System.ArgumentNullException">assemblyPath</exception>
    /// <exception cref="System.IO.FileNotFoundException"></exception>
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.ControlAppDomain)]
    public bool Add(string assemblyPath)
    {
      if (assemblyPath == null)
        throw new ArgumentNullException("assemblyPath");

      FileInfo assemblyFile = new FileInfo(assemblyPath);
      if (!assemblyFile.Exists)
        throw new FileNotFoundException();

      try
      {
        using (AssemblyReflectionManager reflectionManager = new AssemblyReflectionManager())
        {
          lock (this.assemblyPaths)
          {
            if (this.pathAssembly.ContainsKey(assemblyPath))
              throw new ArgumentException("Assembly already added");

            if (reflectionManager.LoadAssembly(assemblyFile.FullName, Guid.NewGuid().ToString()))
            {
              var assemblyName = reflectionManager.Reflect(assemblyFile.FullName, assembly => assembly.FullName);
              AddPathToName(assemblyName, assemblyFile.FullName);

              if (this.AssemblyAdded != null)
                this.AssemblyAdded(this, new AssemblyAddedEventArgs(assemblyFile.FullName, reflectionManager));

              return true;
            }
          }
        }
      }
      catch (BadImageFormatException) { }
      catch (FileLoadException) { }

      return false;
    }

    private void AddPathToName(string assemblyName, string path)
    {
      List<string> paths;
      if (this.assemblyPaths.TryGetValue(assemblyName, out paths) == false)
      {
        paths = new List<string>();
        this.assemblyPaths.Add(assemblyName, paths);
      }
      paths.Add(path);
      pathAssembly.Add(path, assemblyName);
    }

    /// <summary>
    /// Removes the specified assembly file.
    /// </summary>
    /// <param name="assemblyPath">The assembly path.</param>
    /// <exception cref="System.ArgumentNullException">assemblyPath</exception>
    public void Remove(string assemblyPath)
    {
      if (assemblyPath == null)
        throw new ArgumentNullException("assemblyPath");

      lock (this.assemblyPaths)
      {
        string assemblyName;
        if (this.pathAssembly.TryGetValue(assemblyPath, out assemblyName) == false)
          return;

        this.pathAssembly.Remove(assemblyPath);

        List<string> paths;
        if (this.assemblyPaths.TryGetValue(assemblyName, out paths))
        {
          paths.Remove(assemblyPath);
          if (paths.Count == 0)
            this.assemblyPaths.Remove(assemblyName);

          if (this.AssemblyRemoved != null)
            this.AssemblyRemoved(this, new AssemblyRemovedEventArgs(assemblyPath));
        }
      }
    }

    /// <summary>
    /// Connects a PluginDirectory to this container. Changes to assemblies reported by the plugin directory will reflected in the container.
    /// </summary>
    /// <param name="dir">The plugin directory.</param>
    /// <exception cref="System.ArgumentNullException">dir</exception>
    /// <exception cref="System.ArgumentException">Directory already added</exception>
    public void AddDir(IPluginDirectory dir)
    {
      if (dir == null)
        throw new ArgumentNullException("dir");

      if (this.directories.Contains(dir))
        throw new ArgumentException("Directory already added");

      dir.FileFound += this.PluginDirectoryFileFound;
      dir.FileLost += this.PluginDirFileLost;
      this.directories.Add(dir);
    }

    /// <summary>
    /// Removes a plugin directory from the container. Changes reported from the plugin directory will no longer be reflected in the container.
    /// </summary>
    /// <param name="dir">The dir.</param>
    /// <exception cref="System.ArgumentNullException">dir</exception>
    /// <exception cref="System.ArgumentException">Unknown directory</exception>
    public void RemoveDir(IPluginDirectory dir)
    {
      if (dir == null)
        throw new ArgumentNullException("dir");

      if (!this.directories.Contains(dir))
        throw new ArgumentException("Unknown directory");

      this.directories.Remove(dir);
      dir.FileFound -= this.PluginDirectoryFileFound;
      dir.FileLost -= this.PluginDirFileLost;
    }

    private void PluginDirectoryFileFound(object sender, PluginDirectoryEventArgs e)
    {
      this.Add(e.FullName);
    }

    private void PluginDirFileLost(object sender, PluginDirectoryEventArgs e)
    {
      this.Remove(e.FullName);
    }

    /// <summary>
    /// Fetches the specified assembly by it's full name.
    /// </summary>
    /// <param name="assemblyFullName">Full name of the assembly.</param>
    /// <returns>
    /// Assembly as byte array or null if the assembly was not found in the repository.
    /// </returns>
    public byte[] Fetch(string assemblyFullName)
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
          buffer = File.ReadAllBytes(assemblyFile.FullName);
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
