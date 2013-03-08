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
  using System.Reflection;
  using System.Security.Permissions;

  /// <summary>
  /// Interface for reporting changes in a collection of assemblies
  /// </summary>
  public interface IAssemblySource
  {
    /// <summary>
    /// Occurs when an assembly is added.
    /// </summary>
    event EventHandler<AssemblyAddedEventArgs> AssemblyAdded;

    /// <summary>
    /// Occurs when an assembly removed.
    /// </summary>
    event EventHandler<AssemblyRemovedEventArgs> AssemblyRemoved;
  }


  /// <summary>
  /// IAssemblySource, Assembly added event arguments.
  /// </summary>
  public class AssemblyAddedEventArgs : EventArgs
  {
    AssemblyReflectionManager reflectionManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyAddedEventArgs"/> class.
    /// </summary>
    /// <param name="id">The assembly id.</param>
    /// <param name="reflectionManager">The reflection manager.</param>
    public AssemblyAddedEventArgs(string id, AssemblyReflectionManager reflectionManager)
    {
      this.AssemblyId = id;
      this.reflectionManager = reflectionManager;
    }

    /// <summary>
    /// Gets the IAssemblySource identifier for the assembly.
    /// </summary>
    /// <value>
    /// The assembly id.
    /// </value>
    public string AssemblyId { get; private set; }

    /// <summary>
    /// Method for performing reflection on the added assembly. 
    /// <remarks>
    /// The assembly is loaded as ReflectionOnly.
    /// The reflector function will be run inside another AppDomain.
    /// </remarks>
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="reflector">The reflector function.</param>
    /// <returns>The return value from the reflector function.</returns>
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.ControlAppDomain)]
    public T Reflect<T>(Func<Assembly, T> reflector)
    {
      return reflectionManager.Reflect(AssemblyId, reflector);
    }
  }


  /// <summary>
  /// IAssemblySource, Assembly removed event arguments.
  /// </summary>
  public class AssemblyRemovedEventArgs : EventArgs
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AssemblyRemovedEventArgs"/> class.
    /// </summary>
    /// <param name="assemblyId">The assembly id.</param>
    public AssemblyRemovedEventArgs(string assemblyId)
    {
      this.AssemblyId = assemblyId;
    }

    /// <summary>
    /// Gets the IAssemblySource identifier for the assembly.
    /// </summary>
    /// <value>
    /// The assembly id.
    /// </value>
    public string AssemblyId { get; private set; }
  }
}
