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
  /// Logger factory that use an internal ILoggerFactory for ILog creation.
  /// If the created ILog isn't serializable then it then wraps the log 
  /// inside a ProxyLog object that can travel across AppDomains.
  /// </summary>
  internal class ProxyLoggerFactory : MarshalByRefObject, ILoggerFactory
  {
    ILoggerFactory factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProxyLoggerFactory"/> class.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <exception cref="System.ArgumentNullException">factory</exception>
    public ProxyLoggerFactory(ILoggerFactory factory)
    {
      if (factory == null)
        throw new ArgumentNullException("factory");

      this.factory = factory;
    }

    /// <summary>
    /// Fetches a named logger.
    /// </summary>
    /// <param name="name">The logger name.</param>
    /// <returns>
    /// A logger
    /// </returns>
    public ILog GetLog(string name)
    {
      ILog log = this.factory.GetLog(name);
      if (log is MarshalByRefObject)
        return log;
      else
        return new ProxyLog(log);
    }
  }
}
