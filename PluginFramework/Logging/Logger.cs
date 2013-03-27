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
  /// Static class for managing and calling logger factories.
  /// </summary>
  public class Logger : ILogger
  {
    private static Logger singleton = new Logger();

    /// <summary>
    /// Gets the global logger.
    /// </summary>
    /// <value>
    /// The global logger.
    /// </value>
    public static Logger Singleton { get { return singleton; } }

    private ILoggerFactory factory;

    /// <summary>
    /// Prevents a default instance of the <see cref="Logger"/> class from being created.
    /// </summary>
    internal Logger()
    {
      factory = new TraceLoggerFactory();
    }

    /// <summary>
    /// Gets or sets the logger factory.
    /// </summary>
    /// <value>
    /// The logger factory.
    /// </value>
    /// <exception cref="System.ArgumentNullException">value</exception>
    public ILoggerFactory LoggerFactory
    {
      get 
      { 
        return this.factory; 
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");
        this.factory = value;
      }
    }

    /// <summary>
    /// Configures the PluginFramework logger
    /// </summary>
    /// <param name="action">The action.</param>
    public static void Configure(Action<Configurator> action)
    {
      if (action == null)
        throw new ArgumentNullException("action");

      action(new Configurator());
    }

    /// <summary>
    /// Gets a named log.
    /// </summary>
    /// <param name="name">The log name.</param>
    /// <returns>
    /// A named log
    /// </returns>
    public ILog GetLog(string name)
    {
      return this.LoggerFactory.GetLog(name);
    }
  }
}
