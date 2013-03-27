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
  using System.Globalization;

  /// <summary>
  /// Interface for a log.
  /// </summary>
  public interface ILog
  {
    /// <summary>
    /// Gets the name of the log.
    /// </summary>
    /// <value>
    /// The log name.
    /// </value>
    string Name { get; }

    /// <summary>
    /// Writes a debug level message.
    /// </summary>
    /// <param name="message">The message.</param>
    void Debug(string message);

    /// <summary>
    /// Writes a formatted debug level message.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="format">The format.</param>
    /// <param name="args">The format args.</param>
    void Debug(IFormatProvider formatProvider, string format, params object[] args);

    /// <summary>
    /// Writes a info level message.
    /// </summary>
    /// <param name="message">The message.</param>
    void Info(string message);

    /// <summary>
    /// Writes a formatted info level message.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="format">The format.</param>
    /// <param name="args">The format args.</param>
    void Info(IFormatProvider formatProvider, string format, params object[] args);

    /// <summary>
    /// Writes a warning level message.
    /// </summary>
    /// <param name="message">The message.</param>
    void Warn(string message);

    /// <summary>
    /// Writes a formatted warn level message.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="format">The format.</param>
    /// <param name="args">The format args.</param>
    void Warn(IFormatProvider formatProvider, string format, params object[] args);

    /// <summary>
    /// Writes a error level message.
    /// </summary>
    /// <param name="message">The message.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error")] // Just following standard of other loggers
    void Error(string message);

    /// <summary>
    /// Writes a formatted error level message.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="format">The format.</param>
    /// <param name="args">The format args.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error")] // Just following standard of other loggers
    void Error(IFormatProvider formatProvider, string format, params object[] args);

    /// <summary>
    /// Writes a fatal error level message.
    /// </summary>
    /// <param name="message">The message.</param>
    void Fatal(string message);

    /// <summary>
    /// Writes a formatted fatal level message.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <param name="format">The format.</param>
    /// <param name="args">The format args.</param>
    void Fatal(IFormatProvider formatProvider, string format, params object[] args);
  }

  /// <summary>
  /// Helper Extension to ILog
  /// </summary>
  public static class ILogExtensions
  {
    /// <summary>
    /// Writes a formatted debug message using InvariantCulture FormatProvider
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    public static void Debug(this ILog log, string format, params object[] args)
    {
      if (log == null)
        throw new ArgumentNullException("log");

      log.Debug(CultureInfo.InvariantCulture, format, args);
    }

    /// <summary>
    /// Writes a formatted info message using InvariantCulture FormatProvider
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    public static void Info(this ILog log, string format, params object[] args)
    {
      if (log == null)
        throw new ArgumentNullException("log");

      log.Info(CultureInfo.InvariantCulture, format, args);
    }

    /// <summary>
    /// Writes a formatted warning message using InvariantCulture FormatProvider
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    public static void Warn(this ILog log, string format, params object[] args)
    {
      if (log == null)
        throw new ArgumentNullException("log");

      log.Warn(CultureInfo.InvariantCulture, format, args);
    }

    /// <summary>
    /// Writes a formatted error message using InvariantCulture FormatProvider
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    public static void Error(this ILog log, string format, params object[] args)
    {
      if (log == null)
        throw new ArgumentNullException("log");

      log.Error(CultureInfo.InvariantCulture, format, args);
    }

    /// <summary>
    /// Writes a formatted fatal error message using InvariantCulture FormatProvider
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    public static void Fatal(this ILog log, string format, params object[] args)
    {
      if (log == null)
        throw new ArgumentNullException("log");

      log.Fatal(CultureInfo.InvariantCulture, format, args);
    }
  }
}
