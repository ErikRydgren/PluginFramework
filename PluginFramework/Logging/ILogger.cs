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
namespace PluginFramework.Logging
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  /// <summary>
  /// Interface for a logger
  /// </summary>
  internal interface ILogger
  {
    /// <summary>
    /// Gets or sets the logger factory.
    /// </summary>
    /// <value>
    /// The logger factory.
    /// </value>
    ILoggerFactory LoggerFactory { get; set; }

    /// <summary>
    /// Gets a named log.
    /// </summary>
    /// <param name="name">The log name.</param>
    /// <returns>A named log</returns>
    ILog GetLog(string name);
  }

  /// <summary>
  /// Helper extensions for the ILogger interface
  /// </summary>
  public static class ILoggerExtensions
  {
    /// <summary>
    /// Gets a log for the specified type.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="type">The type.</param>
    /// <returns>
    /// A named logger
    /// </returns>
    /// <exception cref="System.ArgumentNullException">type</exception>
    internal static ILog GetLog(this ILogger logger, Type type)
    {
      if (type == null)
        throw new ArgumentNullException("type");

      return logger.GetLog(GetTypeName(type));
    }

    /// <summary>
    /// Gets a log for the specified object.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="logged">The object to log.</param>
    /// <returns>A named logger</returns>
    /// <exception cref="System.ArgumentNullException">logged</exception>
    internal static ILog GetLog(this ILogger logger, object logged)
    {
      if (logged == null)
        throw new ArgumentNullException("logged");

      return logger.GetLog(logged.GetType());
    }

    private static string GetTypeName(Type type)
    {
      return type.FullName;
    }
  }

}
