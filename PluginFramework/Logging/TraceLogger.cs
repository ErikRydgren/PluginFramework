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

  internal class TraceLogger : MarshalByRefObject, ILog
  {
    private string name;

    public TraceLogger(string name)
    {
      this.name = name;
      this.Trace = new TraceWrapper();
    }

    internal TraceLogger(string name, ITrace trace)
    {
      this.name = name;
      this.Trace = trace;
    }

    public string Name
    {
      get { return this.name; }
    }

    internal ITrace Trace { get; set; }

    public void Debug(string message)
    {
      this.Trace.TraceInformation("[{0}] {1} {2}", "DEBUG", this.name, message);
    }

    public void Info(string message)
    {
      this.Trace.TraceInformation("[{0}] {1} {2}", "INFO", this.name, message);
    }

    public void Warn(string message)
    {
      this.Trace.TraceInformation("[{0}] {1} {2}", "WARN", this.name, message);
    }

    public void Error(string message)
    {
      this.Trace.TraceError("[{0}] {1} {2}", "ERROR", this.name, message);
    }

    public void Fatal(string message)
    {
      this.Trace.TraceError("[{0}] {1} {2}", "FATAL", this.name, message);
    }

    public void Debug(IFormatProvider formatProvider, string format, params object[] args)
    {
      this.Trace.TraceInformation("[{0}] {1} {2}", "DEBUG", this.name, string.Format(formatProvider, format, args));
    }

    public void Info(IFormatProvider formatProvider, string format, params object[] args)
    {
      this.Trace.TraceInformation("[{0}] {1} {2}", "INFO", this.name, string.Format(formatProvider, format, args));
    }

    public void Warn(IFormatProvider formatProvider, string format, params object[] args)
    {
      this.Trace.TraceInformation("[{0}] {1} {2}", "WARN", this.name, string.Format(formatProvider, format, args));
    }

    public void Error(IFormatProvider formatProvider, string format, params object[] args)
    {
      this.Trace.TraceError("[{0}] {1} {2}", "ERROR", this.name, string.Format(formatProvider, format, args));
    }

    public void Fatal(IFormatProvider formatProvider, string format, params object[] args)
    {
      this.Trace.TraceError("[{0}] {1} {2}", "FATAL", this.name, string.Format(formatProvider, format, args));
    }
  }
}
