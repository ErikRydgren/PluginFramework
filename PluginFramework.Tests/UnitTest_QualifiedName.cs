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
      QualifiedName tested = new QualifiedName(type.AssemblyQualifiedName);
      Assert.AreEqual(type.FullName, tested.TypeFullName);
      Assert.AreEqual(type.Assembly.FullName, tested.AssemblyFullName);
    }

    [TestMethod]
    public void ConstructionWithNullString()
    {
      DoAssert.Throws<ArgumentNullException>(() => new QualifiedName(null) );
    }

    [TestMethod]
    public void ConstructionWithInvalidString()
    {
      DoAssert.Throws<ArgumentException>(() => new QualifiedName("some invalid string") );
    }
    #endregion

    #region ToString
    [TestMethod]
    public void ToStringCheckExpected()
    {
      Type type = this.GetType();
      QualifiedName qn = new QualifiedName(type.AssemblyQualifiedName);
      Assert.AreEqual(type.AssemblyQualifiedName, qn.ToString());
    }
    #endregion

    #region Equals

    [TestMethod]
    public void EqualsEqualToSelf()
    {
      QualifiedName tested = new QualifiedName(typeof(UnitTest_QualifiedName).AssemblyQualifiedName);
      Assert.IsTrue(tested.Equals(tested));
    }

    [TestMethod]
    public void EqualsNotEqualToNull()
    {
      QualifiedName tested = new QualifiedName(typeof(UnitTest_QualifiedName).AssemblyQualifiedName);
      Assert.IsFalse(tested.Equals(null));
    }

    [TestMethod]
    public void EqualsNotEqualToOtherType()
    {
      QualifiedName tested = new QualifiedName(typeof(UnitTest_QualifiedName).AssemblyQualifiedName);
      Assert.IsFalse(tested.Equals("string"));
    }

    [TestMethod]
    public void EqualsEqualToSameName()
    {
      QualifiedName tested = new QualifiedName(typeof(UnitTest_QualifiedName).AssemblyQualifiedName);
      QualifiedName sameName = new QualifiedName(typeof(UnitTest_QualifiedName).AssemblyQualifiedName);
      Assert.IsTrue(tested.Equals(sameName));
    }

    [TestMethod]
    public void EqualsNotEqualToOtherName()
    {
      QualifiedName tested = new QualifiedName(typeof(UnitTest_QualifiedName).AssemblyQualifiedName);
      QualifiedName otherName = new QualifiedName(typeof(string).AssemblyQualifiedName);
      Assert.IsFalse(tested.Equals(otherName));
    }
    #endregion

    #region GetHashCode
    [TestMethod]
    public void GetHashCodeSameObjectSameHash()
    {
      QualifiedName tested = new QualifiedName(typeof(UnitTest_QualifiedName).AssemblyQualifiedName);
      Assert.AreEqual(tested.GetHashCode(), tested.GetHashCode());
    }

    [TestMethod]
    public void GetHashCodeSameNameSameHash()
    {
      QualifiedName tested = new QualifiedName(typeof(UnitTest_QualifiedName).AssemblyQualifiedName);
      QualifiedName sameName = new QualifiedName(typeof(UnitTest_QualifiedName).AssemblyQualifiedName);
      Assert.AreEqual(sameName.GetHashCode(), tested.GetHashCode());
    }

    [TestMethod]
    public void GetHashCodeDifferentNameDifferentHash()
    {
      QualifiedName tested = new QualifiedName(typeof(UnitTest_QualifiedName).AssemblyQualifiedName);
      QualifiedName otherName = new QualifiedName(typeof(string).AssemblyQualifiedName);
      Assert.AreNotEqual(otherName.GetHashCode(), tested.GetHashCode());
    }

    #endregion

    #region Operators

    #region CastFromString
    [TestMethod]
    public void CastFromValidString()
    {
      QualifiedName tested = typeof(UnitTest_QualifiedName).AssemblyQualifiedName;
    }

    [TestMethod]
    public void CastFromInvalidString()
    {
      DoAssert.Throws<ArgumentException>(() => { QualifiedName tested = "invalid string"; });
    }
    #endregion

    #region CastToString
    [TestMethod]
    public void CastToStringValidateResult()
    {
      var type = typeof(UnitTest_QualifiedName);
      string tested = new QualifiedName(type.AssemblyQualifiedName);
      StringAssert.Equals(type.AssemblyQualifiedName, tested);
    }
    #endregion

    #region Equal
    [TestMethod]
    public void EqualTrueIfSameName()
    {
      QualifiedName tested = new QualifiedName(typeof(UnitTest_QualifiedName).AssemblyQualifiedName);
      QualifiedName sameName = new QualifiedName(typeof(UnitTest_QualifiedName).AssemblyQualifiedName);
      Assert.IsTrue(tested == sameName);
    }

    [TestMethod]
    public void EqualFalseIfDifferentName()
    {
      QualifiedName tested = new QualifiedName(typeof(UnitTest_QualifiedName).AssemblyQualifiedName);
      QualifiedName sameName = new QualifiedName(typeof(string).AssemblyQualifiedName);
      Assert.IsFalse(tested == sameName);
    }
    #endregion

    #region NotEqual
    [TestMethod]
    public void NotEqualFalseIfSameName()
    {
      QualifiedName tested = new QualifiedName(typeof(UnitTest_QualifiedName).AssemblyQualifiedName);
      QualifiedName sameName = new QualifiedName(typeof(UnitTest_QualifiedName).AssemblyQualifiedName);
      Assert.IsFalse(tested != sameName);
    }

    [TestMethod]
    public void NotEqualTrueIfDifferentName()
    {
      QualifiedName tested = new QualifiedName(typeof(UnitTest_QualifiedName).AssemblyQualifiedName);
      QualifiedName sameName = new QualifiedName(typeof(string).AssemblyQualifiedName);
      Assert.IsTrue(tested != sameName);
    }
    #endregion


    #endregion
  }
}
