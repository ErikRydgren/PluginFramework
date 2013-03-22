using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;

namespace PluginFramework.Tests
{
  [TestClass]
  public class UnitTest_PluginSettingException
  {
    #region Constructing
    [TestMethod]
    public void ConstructingDefault()
    {
      PluginSettingException ex = new PluginSettingException();
      Assert.IsNotNull(ex);
    }

    [TestMethod]
    public void ConstructionWithMessage()
    {
      var message = "message";
      PluginSettingException ex = new PluginSettingException(message);
      Assert.AreEqual(message, ex.Message);
    }

    [TestMethod]
    public void ConstructingWithFormattedMessage()
    {
      var message = "message {0} {1}";
      var arg1 = 55;
      var arg2 = "arg2";
      var formatted = string.Format(message, arg1, arg2);
      PluginSettingException ex = new PluginSettingException(message, arg1, arg2);
      Assert.AreEqual(formatted, ex.Message);
    }

    [TestMethod]
    public void ConstructingWithMessageAndInnerException()
    {
      Exception inner = new NotImplementedException();
      string message = "message";
      PluginSettingException ex = new PluginSettingException(message, inner);
      Assert.AreEqual(message, ex.Message);
      Assert.AreSame(inner, ex.InnerException);
    }
    #endregion

    #region Serialize
    [TestMethod]
    public void ShouldSerializeAndDeserialize()
    {
      Exception inner = new NotImplementedException();
      string message = "message";
      PluginSettingException toSerialize = new PluginSettingException(message, inner);
      var knownTypes = new Type[] { typeof(NotImplementedException) };
      PluginSettingException deserialized;

      using (var memstream = new MemoryStream())
      {
        XmlTextWriter writer = new XmlTextWriter(memstream, Encoding.UTF8);
        var serializer = new DataContractSerializer(toSerialize.GetType(), knownTypes);
        serializer.WriteObject(writer, toSerialize);
        writer.Flush();

        memstream.Seek(0, SeekOrigin.Begin);
        XmlTextReader reader = new XmlTextReader(memstream);
        deserialized = serializer.ReadObject(reader) as PluginSettingException;
      }

      Assert.AreEqual(toSerialize.Message, deserialized.Message);
      Assert.IsNotNull(deserialized.InnerException);
      Assert.AreEqual(toSerialize.InnerException.Message, deserialized.InnerException.Message);
    }
    #endregion
  }
}
