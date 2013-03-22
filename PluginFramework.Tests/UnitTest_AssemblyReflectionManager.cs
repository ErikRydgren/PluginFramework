using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace PluginFramework.Tests
{
  [TestClass]
  public class UnitTest_AssemblyReflectionManager
  {
    #region LoadAssembly
    [TestMethod]
    public void ShouldBeAbleToLoadExistingAssembly()
    {
      AssemblyReflectionManager tested = new AssemblyReflectionManager();
      Assert.IsTrue(tested.LoadAssembly(GetType().Assembly.Location));
    }

    [TestMethod]
    public void ShouldBeAbleToLoadSeveralAssembliesIntoSameDomain()
    {
      AssemblyReflectionManager tested = new AssemblyReflectionManager();
      Assert.IsTrue(tested.LoadAssembly(GetType().Assembly.Location));
      Assert.IsTrue(tested.LoadAssembly(tested.GetType().Assembly.Location));
    }

    [TestMethod]
    public void ShouldNotBeAbleToLoadSameAssemblyTwiceIntoSameDomain()
    {
      AssemblyReflectionManager tested = new AssemblyReflectionManager();
      Assert.IsTrue(tested.LoadAssembly(GetType().Assembly.Location));
      Assert.IsFalse(tested.LoadAssembly(GetType().Assembly.Location));
    }

    [TestMethod]
    public void ShouldThrowFileNotFoundOnLoadOfNonExistingAssembly()
    {
      AssemblyReflectionManager tested = new AssemblyReflectionManager();
      DoAssert.Throws<FileNotFoundException>(() => tested.LoadAssembly(Guid.NewGuid().ToString() + ".dll"));
    }

    [TestMethod]
    public void ShouldThrowBadImageFormatOnLoadOfInvalidAssembly()
    {
      AssemblyReflectionManager tested = new AssemblyReflectionManager();
      string fileName = Guid.NewGuid().ToString() + ".dll";
      try
      {
        using (var file = System.IO.File.CreateText(fileName))
          file.WriteLine("not assembly data");

        DoAssert.Throws<BadImageFormatException>(() => tested.LoadAssembly(fileName));
      }
      finally
      {
        System.IO.File.Delete(fileName);
      }
    }
    #endregion

    #region Reflect
    [TestMethod]
    public void ReflectRequiresArgumentAssembly()
    {
      AssemblyReflectionManager tested = new AssemblyReflectionManager();
      DoAssert.Throws<ArgumentNullException>(() => tested.Reflect(null, a => a.FullName));
    }

    [TestMethod]
    public void ReflectRequiresArgumentFunc()
    {
      AssemblyReflectionManager tested = new AssemblyReflectionManager();
      DoAssert.Throws<ArgumentNullException>(() => tested.Reflect<int>(GetType().Assembly.Location, null));
    }

    [TestMethod]
    public void ProxyReflectRequiresArgumentFunc()
    {
      AssemblyReflectionProxy tested = new AssemblyReflectionProxy();
      DoAssert.Throws<ArgumentNullException>(() => tested.Reflect<int>(null));
    }

    [TestMethod]
    public void ReflectShouldThrowArgumentExceptionOnUnknownAssembly()
    {
      AssemblyReflectionManager tested = new AssemblyReflectionManager();
      DoAssert.Throws<ArgumentException>(() => tested.Reflect(GetType().Assembly.Location, a => a.FullName));
    }
    #endregion  
  }
}
