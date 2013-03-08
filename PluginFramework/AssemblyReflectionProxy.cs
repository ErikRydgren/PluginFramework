/* This is shamelessly "borrowed" from CodeProject with just slight modification
 * http://www.codeproject.com/Articles/453778/Loading-Assemblies-from-Anywhere-into-a-New-AppDom
 * My thanks to the author!
 */
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
  /// Acts as a proxy for loading and reflecting an assembly inside another AppDomain without having to load the assembly inside the current AppDomain.
  /// </summary>
  public class AssemblyReflectionProxy : MarshalByRefObject
  {
    private string _assemblyPath;

    /// <summary>
    /// Loads the assembly into the current appdomain
    /// </summary>
    /// <param name="assemblyPath">The assembly path.</param>
    public void LoadAssembly(String assemblyPath)
    {
      try
      {
        _assemblyPath = assemblyPath;
        Assembly.ReflectionOnlyLoadFrom(assemblyPath);
      }
      catch (FileNotFoundException)
      {
        // Continue loading assemblies even if an assembly can not be loaded in the new AppDomain.
      }
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
      Assembly loadedAssembly =
          AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies()
              .FirstOrDefault(
                asm => string.Equals(asm.FullName, args.Name,
                    StringComparison.OrdinalIgnoreCase));

      if (loadedAssembly != null)
      {
        return loadedAssembly;
      }

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

  /// <summary>
  /// Simplifies assembly loading and reflection inside a remote AppDomain by using proxies inside the remote AppDomain.
  /// </summary>
  public class AssemblyReflectionManager : IDisposable
  {
    Dictionary<string, AppDomain> _mapDomains = new Dictionary<string, AppDomain>();
    Dictionary<string, AppDomain> _loadedAssemblies = new Dictionary<string, AppDomain>();
    Dictionary<string, AssemblyReflectionProxy> _proxies = new Dictionary<string, AssemblyReflectionProxy>();

    /// <summary>
    /// Loads the assembly.
    /// </summary>
    /// <param name="assemblyPath">The assembly path.</param>
    /// <param name="domainName">Name of the domain.</param>
    /// <returns>True if the assembly could be loaded</returns>
    public bool LoadAssembly(string assemblyPath, string domainName)
    {
      try
      {
        // if the assembly file does not exist then fail
        if (!File.Exists(assemblyPath))
          return false;

        // if the assembly was already loaded then fail
        if (_loadedAssemblies.ContainsKey(assemblyPath))
        {
          return false;
        }

        // check if the appdomain exists, and if not create a new one
        AppDomain appDomain = null;
        if (_mapDomains.ContainsKey(domainName))
        {
          appDomain = _mapDomains[domainName];
        }
        else
        {
          appDomain = CreateChildDomain(AppDomain.CurrentDomain, domainName);
          _mapDomains[domainName] = appDomain;
        }

        // load the assembly in the specified app domain
        Type proxyType = typeof(AssemblyReflectionProxy);
        if (proxyType.Assembly != null)
        {
          var proxy =
              (AssemblyReflectionProxy)appDomain.
                  CreateInstanceFrom(
                  proxyType.Assembly.Location,
                  proxyType.FullName).Unwrap();

          proxy.LoadAssembly(assemblyPath);

          _loadedAssemblies[assemblyPath] = appDomain;
          _proxies[assemblyPath] = proxy;

          return true;
        }
      }
      catch (TypeLoadException) { }
      catch (AppDomainUnloadedException) { }
      catch (BadImageFormatException) { }
      catch (FileLoadException) { }
      catch (FileNotFoundException) { }
      catch (SecurityException) { }

      return false;
    }

    /// <summary>
    /// Unloads the assembly.
    /// </summary>
    /// <param name="assemblyPath">The assembly path.</param>
    /// <returns>
    /// True if the assembly could be unloaded
    /// </returns>
    /// <exception cref="System.ArgumentNullException">assemblyPath</exception>
    public bool UnloadAssembly(string assemblyPath)
    {
      if (assemblyPath == null)
        throw new ArgumentNullException("assemblyPath");

      if (!File.Exists(assemblyPath))
        return false;

      // check if the assembly is found in the internal dictionaries
      if (_loadedAssemblies.ContainsKey(assemblyPath) && _proxies.ContainsKey(assemblyPath))
      {
        // check if there are more assemblies loaded in the same app domain; in this case fail
        AppDomain appDomain = _loadedAssemblies[assemblyPath];
        int count = _loadedAssemblies.Values.Count(a => a == appDomain);
        if (count != 1)
          return false;

        try
        {
          // remove the appdomain from the dictionary and unload it from the process
          _mapDomains.Remove(appDomain.FriendlyName);
          AppDomain.Unload(appDomain);

          // remove the assembly from the dictionaries
          _loadedAssemblies.Remove(assemblyPath);
          _proxies.Remove(assemblyPath);

          return true;
        }
        catch (CannotUnloadAppDomainException)
        {
        }
      }

      return false;
    }

    /// <summary>
    /// Unloads the domain.
    /// </summary>
    /// <param name="domainName">Name of the domain.</param>
    /// <returns>True if the appdomain was unloaded</returns>
    public bool UnloadDomain(string domainName)
    {
      // check the appdomain name is valid
      if (string.IsNullOrEmpty(domainName))
        return false;

      // check we have an instance of the domain
      if (_mapDomains.ContainsKey(domainName))
      {
        try
        {
          var appDomain = _mapDomains[domainName];

          // check the assemblies that are loaded in this app domain
          var assemblies = new List<string>();
          foreach (var kvp in _loadedAssemblies)
          {
            if (kvp.Value == appDomain)
              assemblies.Add(kvp.Key);
          }

          // remove these assemblies from the internal dictionaries
          foreach (var assemblyName in assemblies)
          {
            _loadedAssemblies.Remove(assemblyName);
            _proxies.Remove(assemblyName);
          }

          // remove the appdomain from the dictionary
          _mapDomains.Remove(domainName);

          // unload the appdomain
          AppDomain.Unload(appDomain);

          return true;
        }
        catch (CannotUnloadAppDomainException)
        {
        }
      }

      return false;
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
      // check if the assembly is found in the internal dictionaries
      if (_loadedAssemblies.ContainsKey(assemblyPath) &&
         _proxies.ContainsKey(assemblyPath))
      {
        return _proxies[assemblyPath].Reflect(func);
      }

      return default(TResult);
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
    /// Finalizes an instance of the <see cref="AssemblyReflectionManager"/> class.
    /// </summary>
    ~AssemblyReflectionManager()
    {
      Dispose(false);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        foreach (var appDomain in _mapDomains.Values)
          AppDomain.Unload(appDomain);

        _loadedAssemblies.Clear();
        _proxies.Clear();
        _mapDomains.Clear();
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
}
