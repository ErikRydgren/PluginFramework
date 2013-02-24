using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MassTransit;

namespace PluginFramework.Command
{
  [Serializable]
  public class FetchAssemblyResponse
  {
    public FetchAssemblyResponse(FetchAssembly request)
    {
      this.Request = request;
    }

    public FetchAssembly Request { get; set; }
    public byte[] Bytes { get; set; }
  }
}
