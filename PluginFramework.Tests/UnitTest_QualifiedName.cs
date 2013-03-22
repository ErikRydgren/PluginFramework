using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluginFramework.Tests
{
  [TestClass]
  public sealed class UnitTest_QualifiedName
  {
    #region Construction
    [TestMethod]
    public void ConstructionWithValidAssemblyQualifiedName()
    {
      var type = typeof(UnitTest_QualifiedName);
      QualifiedName tested = type;
      Assert.AreEqual(type.FullName, tested.TypeFullName);
      Assert.AreEqual(type.Assembly.FullName, tested.AssemblyFullName);
    }

    [TestMethod]
    public void ConstructionRequiresTypeFullName()
    {
      var type = typeof(UnitTest_QualifiedName);
      DoAssert.Throws<ArgumentNullException>(() => new QualifiedName(null, type.Assembly.FullName));
    }

    [TestMethod]
    public void ConstructionRequiresAssemblyFullName()
    {
      var type = typeof(UnitTest_QualifiedName);
      DoAssert.Throws<ArgumentNullException>(() => new QualifiedName(type.FullName, null));
    }

    [TestMethod]
    public void ConstructionRequiresType()
    {
      DoAssert.Throws<ArgumentNullException>(() => new QualifiedName(null) );
    }
    #endregion

    #region ToString
    [TestMethod]
    public void ToStringCheckExpected()
    {
      Type type = this.GetType();
      QualifiedName qn = type;
      Assert.AreEqual(type.AssemblyQualifiedName, qn.ToString());
    }
    #endregion

    #region Equals

    [TestMethod]
    public void EqualsEqualToSelf()
    {
      QualifiedName tested = typeof(UnitTest_QualifiedName);
      Assert.IsTrue(tested.Equals(tested));
    }

    [TestMethod]
    public void EqualsNotEqualToNull()
    {
      QualifiedName tested = typeof(UnitTest_QualifiedName);
      Assert.IsFalse(tested.Equals(null));
    }

    [TestMethod]
    public void EqualsNotEqualToOtherType()
    {
      QualifiedName tested = typeof(UnitTest_QualifiedName);
      Assert.IsFalse(tested.Equals("string"));
    }

    [TestMethod]
    public void EqualsEqualToSameName()
    {
      QualifiedName tested = typeof(UnitTest_QualifiedName);
      QualifiedName sameName = typeof(UnitTest_QualifiedName);
      Assert.IsTrue(tested.Equals(sameName));
    }

    [TestMethod]
    public void EqualsNotEqualToOtherName()
    {
      QualifiedName tested = typeof(UnitTest_QualifiedName);
      QualifiedName otherName = typeof(string);
      Assert.IsFalse(tested.Equals(otherName));
    }
    #endregion

    #region GetHashCode
    [TestMethod]
    public void GetHashCodeSameObjectSameHash()
    {
      QualifiedName tested = typeof(UnitTest_QualifiedName);
      Assert.AreEqual(tested.GetHashCode(), tested.GetHashCode());
    }

    [TestMethod]
    public void GetHashCodeSameNameSameHash()
    {
      QualifiedName tested = typeof(UnitTest_QualifiedName);
      QualifiedName sameName = typeof(UnitTest_QualifiedName);
      Assert.AreEqual(sameName.GetHashCode(), tested.GetHashCode());
    }

    [TestMethod]
    public void GetHashCodeDifferentNameDifferentHash()
    {
      QualifiedName tested = typeof(UnitTest_QualifiedName);
      QualifiedName otherName = typeof(string);
      Assert.AreNotEqual(otherName.GetHashCode(), tested.GetHashCode());
    }

    #endregion

    #region Operators

    #region CastToString
    [TestMethod]
    public void CastToStringValidateResult()
    {
      var type = typeof(UnitTest_QualifiedName);
      string tested = new QualifiedName(type);
      StringAssert.Equals(type.AssemblyQualifiedName, tested);
    }
    #endregion

    #region Equal
    [TestMethod]
    public void EqualTrueIfSameName()
    {
      QualifiedName tested = new QualifiedName(typeof(UnitTest_QualifiedName));
      QualifiedName sameName = new QualifiedName(typeof(UnitTest_QualifiedName));
      Assert.IsTrue(tested == sameName);
    }

    [TestMethod]
    public void EqualFalseIfDifferentName()
    {
      QualifiedName tested = new QualifiedName(typeof(UnitTest_QualifiedName));
      QualifiedName sameName = new QualifiedName(typeof(string));
      Assert.IsFalse(tested == sameName);
    }
    #endregion

    #region NotEqual
    [TestMethod]
    public void NotEqualFalseIfSameName()
    {
      QualifiedName tested = new QualifiedName(typeof(UnitTest_QualifiedName));
      QualifiedName sameName = new QualifiedName(typeof(UnitTest_QualifiedName));
      Assert.IsFalse(tested != sameName);
    }

    [TestMethod]
    public void NotEqualTrueIfDifferentName()
    {
      QualifiedName tested = new QualifiedName(typeof(UnitTest_QualifiedName));
      QualifiedName sameName = new QualifiedName(typeof(string));
      Assert.IsTrue(tested != sameName);
    }
    #endregion


    #endregion
  }
}
