using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PluginFramework
{
  /// <summary>
  /// Describes an exception throwed by the PluginFramework
  /// </summary>
  [Serializable]
  public class PluginException : ApplicationException
  {
    public PluginException()
    {
    }

    public PluginException(string message)
      :base(message)
    {
    }

    public PluginException(string message, params object[] args)
      : base(string.Format(message, args))
    {
    }

    public PluginException(Exception innerException, string message, params object[] args)
      : base(string.Format(message, args), innerException)
    {
    }

    public PluginException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
      : base(info, context)
    {
    }
  }
}
