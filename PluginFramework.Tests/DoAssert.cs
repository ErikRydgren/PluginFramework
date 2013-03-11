using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginFramework.Tests
{
  internal class DoAssert
  {
    public static TException Throws<TException>(Action action) where TException : Exception
    {
      try
      {
        action();
      }
      catch (TException ex)
      {
        return ex;
      }
      catch (Exception ex)
      {
        Assert.Fail("Expected exception " + typeof(TException).Name + " was not thrown, got " + ex.GetType().Name + " instead");
      }
      Assert.Fail("Expected exception " + typeof(TException).Name + " was not thrown");

      return null;
    }
  }
}
