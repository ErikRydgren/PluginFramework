// This is shamelessly "borrowed" from CodeProject and modified quite extensivly to suit the needs of PluginFramework
// http://www.codeproject.com/Articles/453778/Loading-Assemblies-from-Anywhere-into-a-New-AppDom
// My thanks to Sacha Barber and Marius Bancila for his derative work and for their genorosity of releasing the 
// code under the CPOL licence (http://www.codeproject.com/info/cpol10.aspx).
// 
// Copyright (c) 2013, Sacha Barber, Marius Bancila, Erik Rydgren, et al. All rights reserved.
// The code below is released under the CPOL licence (http://www.codeproject.com/info/cpol10.aspx)
//
// THIS WORK IS PROVIDED "AS IS", "WHERE IS" AND "AS AVAILABLE", WITHOUT ANY EXPRESS OR IMPLIED WARRANTIES OR CONDITIONS 
// OR GUARANTEES. YOU, THE USER, ASSUME ALL RISK IN ITS USE, INCLUDING COPYRIGHT INFRINGEMENT, PATENT INFRINGEMENT, 
// SUITABILITY, ETC. AUTHOR EXPRESSLY DISCLAIMS ALL EXPRESS, IMPLIED OR STATUTORY WARRANTIES OR CONDITIONS, 
// INCLUDING WITHOUT LIMITATION, WARRANTIES OR CONDITIONS OF MERCHANTABILITY, MERCHANTABLE QUALITY OR FITNESS FOR A 
// PARTICULAR PURPOSE, OR ANY WARRANTY OF TITLE OR NON-INFRINGEMENT, OR THAT THE WORK (OR ANY PORTION THEREOF) IS CORRECT, 
// USEFUL, BUG-FREE OR FREE OF VIRUSES. YOU MUST PASS THIS DISCLAIMER ON WHENEVER YOU DISTRIBUTE THE WORK OR DERIVATIVE WORKS. 
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Security.Permissions;
using System.Security;

namespace PluginFramework
{
  /// <summary>
  /// Simplifies assembly loading and reflection inside a remote AppDomain by using this.proxies inside the remote AppDomain.
  /// </summary>
  public class AssemblyReflectionManager : IDisposable
  {
    AppDomain appDomain;
    Dictionary<string, AssemblyReflectionProxy> proxies;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyReflectionManager"/> class.
    /// </summary>
    public AssemblyReflectionManager()
    {
      this.appDomain = CreateChildDomain(AppDomain.CurrentDomain, Guid.NewGuid().ToString());
      this.proxies = new Dictionary<string, AssemblyReflectionProxy>();
    }

    /// <summary>
    /// Loads the assembly.
    /// </summary>
    /// <param name="assemblyPath">The assembly path.</param>
    /// <returns>True if the assembly could be loaded</returns>
    public bool LoadAssembly(string assemblyPath)
    {
      if (this.proxies.ContainsKey(assemblyPath))
        return false;

      var proxy =
          (AssemblyReflectionProxy)this.appDomain.
              CreateInstanceFromAndUnwrap(
              typeof(AssemblyReflectionProxy).Assembly.Location,
              typeof(AssemblyReflectionProxy).FullName);

      proxy.LoadAssembly(assemblyPath);

      this.proxies[assemblyPath] = proxy;

      return true;
    }

    /// <summary>
    /// Reflects the specified assembly path.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="assemblyPath">The assembly path.</param>
    /// <param name="func">The reflection function</param>
    /// <returns>The returned value from func</returns>
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.ControlAppDomain)]
    public TResult Reflect<TResult>(string assemblyPath, Func<Assembly, TResult> func)
    {
      if (assemblyPath == null)
        throw new ArgumentNullException("assemblyPath");

      if (func == null)
        throw new ArgumentNullException("func");

      AssemblyReflectionProxy proxy;
      if (!this.proxies.TryGetValue(assemblyPath, out proxy))
        throw new ArgumentException(Resources.UnknownAssembly);
    
      return proxy.Reflect(func);
    }

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
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.proxies != null)
        {
          this.proxies.Clear();
          this.proxies = null;
        }

        if (this.appDomain != null)
        {
          AppDomain.Unload(this.appDomain);
          this.appDomain = null;
        }
      }
    }

    /// <summary>
    /// Creates the child domain.
    /// </summary>
    /// <param name="parentDomain">The parent domain.</param>
    /// <param name="domainName">Name of the domain.</param>
    /// <returns>The created appdomain</returns>
    private static AppDomain CreateChildDomain(AppDomain parentDomain, string domainName)
    {
      Evidence evidence = new Evidence(parentDomain.Evidence);
      AppDomainSetup setup = parentDomain.SetupInformation;
      return AppDomain.CreateDomain(domainName, evidence, setup);
    }
  }

  /// <summary>
  /// Acts as a proxy for loading and reflecting an assembly inside another AppDomain without having to load the assembly inside the current AppDomain.
  /// </summary>
  internal class AssemblyReflectionProxy : MarshalByRefObject
  {
    private string _assemblyPath;

    /// <summary>
    /// Loads the assembly into the current appdomain
    /// </summary>
    /// <param name="assemblyPath">The assembly path.</param>
    public void LoadAssembly(String assemblyPath)
    {
      _assemblyPath = assemblyPath;
      Assembly.ReflectionOnlyLoadFrom(assemblyPath);
    }

    /// <summary>
    /// Reflects the assembly using the specified func.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="func">The reflection function</param>
    /// <returns>
    /// The value returned from func
    /// </returns>
    /// <exception cref="System.ArgumentNullException">func</exception>
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.ControlAppDomain)]
    public TResult Reflect<TResult>(Func<Assembly, TResult> func)
    {
      if (func == null)
        throw new ArgumentNullException("func");

      DirectoryInfo directory = new FileInfo(_assemblyPath).Directory;
      ResolveEventHandler resolveEventHandler =
          (s, e) =>
          {
            return OnReflectionOnlyResolve(
                e, directory);
          };

      AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += resolveEventHandler;

      var assembly = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies().FirstOrDefault(a => StringComparer.InvariantCulture.Compare(a.Location, _assemblyPath) == 0);

      var result = func(assembly);

      AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= resolveEventHandler;

      return result;
    }

    /// <summary>
    /// Raises the <see cref="E:ReflectionOnlyResolve" /> event.
    /// </summary>
    /// <param name="args">The <see cref="ResolveEventArgs"/> instance containing the event data.</param>
    /// <param name="directory">The directory.</param>
    /// <returns>The resolved assembly</returns>
    private static Assembly OnReflectionOnlyResolve(ResolveEventArgs args, DirectoryInfo directory)
    {
      AssemblyName assemblyName =
          new AssemblyName(args.Name);
      string dependentAssemblyFilename =
          Path.Combine(directory.FullName,
          assemblyName.Name + ".dll");

      if (File.Exists(dependentAssemblyFilename))
      {
        return Assembly.ReflectionOnlyLoadFrom(
            dependentAssemblyFilename);
      }
      return Assembly.ReflectionOnlyLoad(args.Name);
    }
  }
}
