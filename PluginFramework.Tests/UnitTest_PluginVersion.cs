using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Threading;

namespace PluginFramework.Tests
{
  /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  [TestClass]
  public sealed class UnitTest_PluginVersion
  {
    #region Construction

    [TestMethod]
    public void ConstructionDefault()
    {
      PluginVersion version = new PluginVersion();
      Assert.AreEqual(0, version.Major);
      Assert.AreEqual(0, version.Minor);
    }

    [TestMethod]
    public void ConstructionWithMajorAndMinor()
    {
      PluginVersion version = new PluginVersion(55, 66);
      Assert.AreEqual(55, version.Major);
      Assert.AreEqual(66, version.Minor);
    }

    [TestMethod]
    public void ConstructionWithValidVersionString()
    {
      PluginVersion version = new PluginVersion("55.66");
      Assert.AreEqual(55, version.Major);
      Assert.AreEqual(66, version.Minor);
    }

    [TestMethod]
    public void ConstructionMustProvideVersionString()
    {
      DoAssert.Throws<ArgumentNullException>(() => new PluginVersion(null) );
    }

    [TestMethod]
    public void ConstructionVersionStringMustNotContainAphaChar()
    {      
      DoAssert.Throws<ArgumentException>(() => new PluginVersion("55t.66") );
    }

    [TestMethod]
    public void ConstructionVersionStringMustContainDot()
    {
      DoAssert.Throws<ArgumentException>(() => new PluginVersion("55") );
    }
    #endregion

    #region CompareTo
    [TestMethod]
    public void CompareToSelf()
    {
      PluginVersion version = new PluginVersion(55, 66);
      Assert.AreEqual(0, version.CompareTo(version));
    }

    [TestMethod]
    public void CompareToSameVersion()
    {
      PluginVersion version = new PluginVersion(55, 66);
      PluginVersion version2 = new PluginVersion(55, 66);
      Assert.AreEqual(0, version.CompareTo(version2));
    }

    [TestMethod]
    public void CompareToLesserMinor()
    {
      PluginVersion version = new PluginVersion(55, 66);
      PluginVersion version2 = new PluginVersion(55, 65);
      Assert.IsTrue(version2.CompareTo(version) < 0);
    }

    [TestMethod]
    public void CompareToLesserMajor()
    {
      PluginVersion version = new PluginVersion(55, 66);
      PluginVersion version2 = new PluginVersion(54, 66);
      Assert.IsTrue(version2.CompareTo(version) < 0);
    }

    [TestMethod]
    public void CompareToGreaterMinor()
    {
      PluginVersion version = new PluginVersion(55, 66);
      PluginVersion version2 = new PluginVersion(55, 67);
      Assert.IsTrue(version2.CompareTo(version) > 0);
    }

    [TestMethod]
    public void CompareToGreaterMajor()
    {
      PluginVersion version = new PluginVersion(55, 66);
      PluginVersion version2 = new PluginVersion(56, 66);
      Assert.IsTrue(version2.CompareTo(version) > 0);
    }

    [TestMethod]
    public void CompareToAsComparable()
    {
      IComparable comparable = new PluginVersion(55, 66);
      object versionObject = new PluginVersion(56, 66);
      Assert.IsTrue(comparable.CompareTo(versionObject) < 0);
    }
    #endregion

    #region Equals
    [TestMethod]
    public void EqualsSameVersion()
    {
      PluginVersion version = new PluginVersion(55, 66);
      PluginVersion version2 = new PluginVersion(55, 66);
      Assert.IsTrue(version.Equals(version2));
    }

    [TestMethod]
    public void EqualsOtherVersion()
    {
      PluginVersion version = new PluginVersion(55, 66);
      PluginVersion version2 = new PluginVersion(88, 66);
      Assert.IsFalse(version.Equals(version2));
    }

    [TestMethod]
    public void EqualsSameVersionObject()
    {
      PluginVersion version = new PluginVersion(55, 66);
      object version2 = new PluginVersion(55, 66);
      Assert.IsTrue(version.Equals(version2));
    }

    [TestMethod]
    public void EqualsOtherVersionObject()
    {
      PluginVersion version = new PluginVersion(55, 66);
      object version2 = new PluginVersion(88, 66);
      Assert.IsFalse(version.Equals(version2));
    }

    [TestMethod]
    public void EqualsOtherType()
    {
      PluginVersion version = new PluginVersion(55, 66);
      object version2 = "some string";
      Assert.IsFalse(version.Equals(version2));
    }
    #endregion

    #region GetHashCode
    [TestMethod]
    public void GetHashCodeSameObjectSameHash()
    {
      PluginVersion version = new PluginVersion(55, 66);
      Assert.AreEqual(version.GetHashCode(), version.GetHashCode());
    }

    [TestMethod]
    public void GetHashCodeSameVersionSameHash()
    {
      PluginVersion version = new PluginVersion(55, 66);
      var hash = new PluginVersion(55, 66).GetHashCode();
      Assert.AreEqual(hash, version.GetHashCode());
    }

    [TestMethod]
    public void GetHashCodeDifferentVersionDifferentHash()
    {
      PluginVersion version = new PluginVersion(55, 66);
      var hash = new PluginVersion(77, 88);
      Assert.AreNotEqual(hash, version.GetHashCode());
    }

    #endregion

    #region ToString
    [TestMethod]
    public void ToStringCheckResult()
    {
      PluginVersion version = new PluginVersion(55,66);
      StringAssert.Equals("55.66", version.ToString());
    }

    #endregion

    #region operators

    #region LessThen
    [TestMethod]
    public void OperatorLessThenIfLesser()
    {
      PluginVersion version1 = new PluginVersion("1.0");
      PluginVersion version2 = new PluginVersion("2.0");
      Assert.IsTrue(version1 < version2);
    }

    [TestMethod]
    public void OperatorLessThenIfNotLesser()
    {
      PluginVersion version1 = new PluginVersion("1.0");
      PluginVersion version2 = new PluginVersion("1.0");
      Assert.IsFalse(version1 < version2);
    }
    #endregion

    #region LessThenOrEqual
    [TestMethod]
    public void OperatorLessThenOrEqualIfLesser()
    {
      PluginVersion version1 = new PluginVersion("1.0");
      PluginVersion version2 = new PluginVersion("2.0");
      Assert.IsTrue(version1 <= version2);
    }

    [TestMethod]
    public void OperatorLessThenOrEqualIfEqual()
    {
      PluginVersion version1 = new PluginVersion("1.0");
      PluginVersion version2 = new PluginVersion("1.0");
      Assert.IsTrue(version1 <= version2);
    }

    [TestMethod]
    public void OperatorLessThenOrEqualIfGreater()
    {
      PluginVersion version1 = new PluginVersion("2.0");
      PluginVersion version2 = new PluginVersion("1.0");
      Assert.IsFalse(version1 <= version2);
    }
    #endregion

    #region GreaterThen
    [TestMethod]
    public void OperatorGreaterThenIfGreater()
    {
      PluginVersion version1 = new PluginVersion("2.0");
      PluginVersion version2 = new PluginVersion("1.0");
      Assert.IsTrue(version1 > version2);
    }

    [TestMethod]
    public void OperatorGreaterThenIfNotGreater()
    {
      PluginVersion version1 = new PluginVersion("1.0");
      PluginVersion version2 = new PluginVersion("1.0");
      Assert.IsFalse(version1 > version2);
    }
    #endregion

    #region GreaterThenOrEqual
    [TestMethod]
    public void OperatorGreaterThenOrEqualIfGreater()
    {
      PluginVersion version1 = new PluginVersion("2.0");
      PluginVersion version2 = new PluginVersion("1.0");
      Assert.IsTrue(version1 >= version2);
    }

    [TestMethod]
    public void OperatorGreaterThenOrEqualIfEqual()
    {
      PluginVersion version1 = new PluginVersion("1.0");
      PluginVersion version2 = new PluginVersion("1.0");
      Assert.IsTrue(version1 >= version2);
    }

    [TestMethod]
    public void OperatorGreaterThenOrEqualIfLesser()
    {
      PluginVersion version1 = new PluginVersion("1.0");
      PluginVersion version2 = new PluginVersion("2.0");
      Assert.IsFalse(version1 >= version2);
    }
    #endregion

    #region Equal
    [TestMethod]
    public void EqualIfEqual()
    {
      PluginVersion version1 = new PluginVersion("1.0");
      PluginVersion version2 = new PluginVersion("1.0");
      Assert.IsTrue(version1 == version2);
    }

    [TestMethod]
    public void EqualIfNotEqual()
    {
      PluginVersion version1 = new PluginVersion("1.0");
      PluginVersion version2 = new PluginVersion("2.0");
      Assert.IsFalse(version1 == version2);
    }
    #endregion

    #region NotEqual
    [TestMethod]
    public void NotEqualIfNotEqual()
    {
      PluginVersion version1 = new PluginVersion("1.0");
      PluginVersion version2 = new PluginVersion("2.0");
      Assert.IsTrue(version1 != version2);
    }

    [TestMethod]
    public void NotEqualIfEqual()
    {
      PluginVersion version1 = new PluginVersion("1.0");
      PluginVersion version2 = new PluginVersion("1.0");
      Assert.IsFalse(version1 != version2);
    }
    #endregion

    #endregion
  }
}
