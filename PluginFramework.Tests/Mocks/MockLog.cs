using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginFramework.Logging;
using System.Collections;

namespace PluginFramework.Tests.Mocks
{
  public class MockLog : ILog, IEnumerable<MockLog.LogEvent>
  {
    public enum Level
    {
      Debug,
      Info,
      Warn,
      Error,
      Fatal,
    };

    public class LogEvent
    {
      public LogEvent(Level level, string message)
      {
        this.Level = level;
        this.Message = message;
      }

      public Level Level { get; private set; }
      public string Message { get; private set; }
    }

    List<LogEvent> logEvents;

    public MockLog()
    {
      this.logEvents = new List<LogEvent>();
    }

    public MockLog(Type type)
      : this()
    {
      this.Name = type.FullName;
    }

    internal MockLog(ILogWriter logWriter)
      : this(logWriter.GetType())
    {
      logWriter.Log = new ProxyLog(this);
    }

    public string Name { get; set; }

    public void Debug(string message)
    {
      this.logEvents.Add(new LogEvent(Level.Debug, message));
    }

    public void Info(string message)
    {
      this.logEvents.Add(new LogEvent(Level.Info, message));
    }

    public void Warn(string message)
    {
      this.logEvents.Add(new LogEvent(Level.Warn, message));
    }

    public void Error(string message)
    {
      this.logEvents.Add(new LogEvent(Level.Error, message));
    }

    public void Fatal(string message)
    {
      this.logEvents.Add(new LogEvent(Level.Fatal, message));
    }

    public void Debug(IFormatProvider formatProvider, string format, params object[] args)
    {
      this.logEvents.Add(new LogEvent(Level.Debug, string.Format(formatProvider, format, args)));
    }

    public void Info(IFormatProvider formatProvider, string format, params object[] args)
    {
      this.logEvents.Add(new LogEvent(Level.Info, string.Format(formatProvider, format, args)));
    }

    public void Warn(IFormatProvider formatProvider, string format, params object[] args)
    {
      this.logEvents.Add(new LogEvent(Level.Warn, string.Format(formatProvider, format, args)));
    }

    public void Error(IFormatProvider formatProvider, string format, params object[] args)
    {
      this.logEvents.Add(new LogEvent(Level.Error, string.Format(formatProvider, format, args)));
    }

    public void Fatal(IFormatProvider formatProvider, string format, params object[] args)
    {
      this.logEvents.Add(new LogEvent(Level.Fatal, string.Format(formatProvider, format, args)));
    }

    public IEnumerator<MockLog.LogEvent> GetEnumerator()
    {
      return this.logEvents.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }
}
