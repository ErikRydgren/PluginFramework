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
  using System.Security.Permissions;

  /// <summary>
  /// <see cref="IPluginDirectory"/> event argument
  /// </summary>
  public class PluginDirectoryEventArgs : EventArgs
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginDirectoryEventArgs"/> class.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    public PluginDirectoryEventArgs(string filePath)
    {
      this.FullName = filePath;
    }

    /// <summary>
    /// Gets the full name of the event file.
    /// </summary>
    /// <value>
    /// The full name of the event file
    /// </value>
    public string FullName { get; private set; }
  }

  /// <summary>
  /// Events for found and lost assembly files
  /// </summary>
  public interface IPluginDirectory : IDisposable
  {
    /// <summary>
    /// Gets the path to the watched directory.
    /// </summary>
    /// <value>
    /// The path.
    /// </value>
    string Path { get; }

    /// <summary>
    /// Occurs when a assembly file is found.
    /// </summary>
    event EventHandler<PluginDirectoryEventArgs> FileFound;

    /// <summary>
    /// Occurs when a assembly file is lost.
    /// </summary>
    event EventHandler<PluginDirectoryEventArgs> FileLost;
  }
}
