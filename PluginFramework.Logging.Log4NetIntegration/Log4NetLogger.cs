using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginFramework.Logging.Log4NetIntegration
{
  public class Log4NetLogger : ILog
  {
    internal log4net.ILog log;

    public Log4NetLogger(log4net.ILog log)
    {
      if (log == null)
        throw new ArgumentNullException("log");

      this.log = log;
    }

    public string Name
    {
      get { return this.log.Logger.Name; }
    }

    public void Debug(string message)
    {
      this.log.Debug(message);
    }

    public void Debug(IFormatProvider formatProvider, string format, params object[] args)
    {
      this.log.DebugFormat(formatProvider, format, args);
    }

    public void Info(string message)
    {
      this.log.Info(message);
    }

    public void Info(IFormatProvider formatProvider, string format, params object[] args)
    {
      this.log.InfoFormat(formatProvider, format, args);
    }

    public void Warn(string message)
    {
      this.log.Warn(message);
    }

    public void Warn(IFormatProvider formatProvider, string format, params object[] args)
    {
      this.log.WarnFormat(formatProvider, format, args);
    }

    public void Error(string message)
    {
      this.log.Error(message);
    }

    public void Error(IFormatProvider formatProvider, string format, params object[] args)
    {
      this.log.ErrorFormat(formatProvider, format, args);
    }

    public void Fatal(string message)
    {
      this.log.Fatal(message);
    }

    public void Fatal(IFormatProvider formatProvider, string format, params object[] args)
    {
      this.log.FatalFormat(formatProvider, format, args);
    }
  }
}
