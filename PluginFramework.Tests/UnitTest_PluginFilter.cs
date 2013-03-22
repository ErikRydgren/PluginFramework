using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;

namespace PluginFramework.Tests
{
  [TestClass]
  public sealed class UnitTest_PluginFilter
  {
    PluginDescriptor plugin2;
    PluginDescriptor plugin1;
    PluginDescriptor plugin3;
    List<PluginDescriptor> descriptors;

    [TestInitialize]
    public void TestInit()
    {
      descriptors = new List<PluginDescriptor>();

      plugin1 = new PluginDescriptor()
      {
        QualifiedName = typeof(MockPlugin1),
        Name = "MockPlugin1",
        Version = new PluginVersion("55.66")
      };
      plugin1.Derives.Add(typeof(MockPluginBase));
      plugin1.Derives.Add(typeof(MarshalByRefObject));
      plugin1.Derives.Add(typeof(object));
      plugin1.Interfaces.Add(typeof(IMockPluginInterface1));
      plugin1.InfoValues.Add("validkey", "validvalue");
      plugin1.Settings.Add(new PluginSettingDescriptor() { Name = "SettingName", Required = false, SettingType = typeof(int) });

      plugin2 = new PluginDescriptor()
      {
        QualifiedName = typeof(MockPlugin2),
        Name = "MockPlugin2",
        Version = new PluginVersion("33.88")
      };
      plugin2.Derives.Add(typeof(MarshalByRefObject));
      plugin2.Derives.Add(typeof(object));
      plugin2.Interfaces.Add(typeof(IMockPluginInterface2));
      plugin2.InfoValues.Add("invalidkey", "invalidvalue");
      plugin2.Settings.Add(new PluginSettingDescriptor() { Name = "InvalidSettingName", Required = false, SettingType = typeof(int) });

      plugin3 = new PluginDescriptor()
      {
        QualifiedName = typeof(MockPlugin3),
        Name = "MockPlugin3",
        Version = new PluginVersion("77.88")
      };
      plugin3.Derives.Add(typeof(MarshalByRefObject));
      plugin3.Derives.Add(typeof(object));
      plugin3.Interfaces.Add(typeof(IMockPluginInterface1));
      plugin3.InfoValues.Add("invalidkey", "invalidvalue");
      plugin3.Settings.Add(new PluginSettingDescriptor() { Name = "InvalidSettingName", Required = false, SettingType = typeof(int) });

      descriptors.Add(plugin2);
      descriptors.Add(plugin1);
      descriptors.Add(plugin3);
    }

    #region Constructing

    [TestMethod]
    public void ConstructingDefault()
    {
      PluginFilter tested = new PluginFilter();
      Assert.AreEqual((PluginFilter.FilterOperation)0, tested.Operation);
      Assert.IsNull(tested.OperationData);
      Assert.IsNull(tested.SubFilters);
    }

    #endregion

    #region Combine
    [TestMethod]
    public void CombineBecomesSubFilters()
    {
      var left = PluginFilter.Create.IsNamed("left");
      var right = PluginFilter.Create.IsNamed("right");

      PluginFilter tested = PluginFilter.Combine(PluginFilter.FilterOperation.And, left, right);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.Operation);
      Assert.IsNull(tested.OperationData);
      Assert.IsNotNull(tested.SubFilters);
      Assert.AreEqual(2, tested.SubFilters.Length);
      Assert.AreSame(left, tested.SubFilters[0]);
      Assert.AreSame(right, tested.SubFilters[1]);
    }

    [TestMethod]
    public void CombineOnlyAcceptBooleanOperators()
    {
      PluginFilter.FilterOperation[] valid = new PluginFilter.FilterOperation[] {
        PluginFilter.FilterOperation.And,
        PluginFilter.FilterOperation.Or,
      };

      var invalid = Enum.GetValues(typeof(PluginFilter.FilterOperation)).OfType<PluginFilter.FilterOperation>().Except(valid);

      var left = PluginFilter.Create.IsNamed("left");
      var right = PluginFilter.Create.IsNamed("right");

      foreach (var value in invalid)
      {
        try
        {
          PluginFilter.Combine(value, left, right);
          Assert.Fail();
        }
        catch (ArgumentException) { }
      }
    }

    [TestMethod]
    public void CombineRequiresLeftArgument()
    {
      DoAssert.Throws<ArgumentNullException>(() => PluginFilter.Combine(PluginFilter.FilterOperation.And, null, PluginFilter.Create.IsNamed("right")));
    }

    [TestMethod]
    public void CombineRequiresRightArgument()
    {
      DoAssert.Throws<ArgumentNullException>(() => PluginFilter.Combine(PluginFilter.FilterOperation.And, PluginFilter.Create.IsNamed("left"), null));
    }

    [TestMethod]
    public void CombineShouldMergeIfArgsAreSameOperator()
    {
      var op = PluginFilter.FilterOperation.Or;
      PluginFilter left1 = PluginFilter.Create.IsNamed("left1");
      PluginFilter right1 = PluginFilter.Create.IsNamed("right1");
      PluginFilter combined1 = PluginFilter.Combine(op, left1, right1);

      PluginFilter left2 = PluginFilter.Create.IsNamed("left2");
      PluginFilter right2 = PluginFilter.Create.IsNamed("right2");
      PluginFilter combined2 = PluginFilter.Combine(op, left2, right2);

      PluginFilter tested = PluginFilter.Combine(op, combined1, combined2);

      Assert.AreEqual(op, tested.Operation);
      Assert.AreEqual(4, tested.SubFilters.Length);
      Assert.IsTrue(tested.SubFilters.Contains(left1));
      Assert.IsTrue(tested.SubFilters.Contains(right1));
      Assert.IsTrue(tested.SubFilters.Contains(left2));
      Assert.IsTrue(tested.SubFilters.Contains(right2));
    }

    [TestMethod]
    public void CombineShouldMergeIfLeftIsSameOperator()
    {
      var op = PluginFilter.FilterOperation.Or;
      PluginFilter left1 = PluginFilter.Create.IsNamed("left1");
      PluginFilter right1 = PluginFilter.Create.IsNamed("right1");
      PluginFilter combined1 = PluginFilter.Combine(op, left1, right1);
      PluginFilter right2 = PluginFilter.Create.IsNamed("right2");

      PluginFilter tested = PluginFilter.Combine(op, combined1, right2);
      Assert.AreEqual(op, tested.Operation);
      Assert.AreEqual(3, tested.SubFilters.Length);
      Assert.IsTrue(tested.SubFilters.Contains(left1));
      Assert.IsTrue(tested.SubFilters.Contains(right1));
      Assert.IsTrue(tested.SubFilters.Contains(right2));
    }

    [TestMethod]
    public void CombineShouldMergeIfRightIsSameOperator()
    {
      var op = PluginFilter.FilterOperation.Or;
      PluginFilter left1 = PluginFilter.Create.IsNamed("right2");

      PluginFilter left2 = PluginFilter.Create.IsNamed("left1");
      PluginFilter right2 = PluginFilter.Create.IsNamed("right1");
      PluginFilter combined2 = PluginFilter.Combine(op, left2, right2);

      PluginFilter tested = PluginFilter.Combine(op, left1, combined2);
      Assert.AreEqual(op, tested.Operation);
      Assert.AreEqual(3, tested.SubFilters.Length);
      Assert.IsTrue(tested.SubFilters.Contains(left1));
      Assert.IsTrue(tested.SubFilters.Contains(left2));
      Assert.IsTrue(tested.SubFilters.Contains(right2));
    }
    #endregion

    #region And
    [TestMethod]
    public void AndRequiresArgument()
    {
      PluginFilter tested = PluginFilter.Create.HasVersion("1.0");
      DoAssert.Throws<ArgumentNullException>(() => tested.And(null));
    }

    [TestMethod]
    public void AndCreatesAndOperatorWithSubFilters()
    {
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      PluginFilter added = PluginFilter.Create.HasInfo("added");
      PluginFilter tested = original.And(added);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.Operation);
      Assert.IsNull(tested.OperationData);
      Assert.AreEqual(2, tested.SubFilters.Length);
      Assert.IsTrue(tested.SubFilters.Contains(original));
      Assert.IsTrue(tested.SubFilters.Contains(added));
    }
    #endregion

    #region Or
    [TestMethod]
    public void OrRequiresArgument()
    {
      PluginFilter tested = PluginFilter.Create.HasVersion("1.0");
      DoAssert.Throws<ArgumentNullException>(() => tested.Or(null));
    }

    [TestMethod]
    public void OrCreatesOrOperatorWithSubFilters()
    {
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      PluginFilter added = PluginFilter.Create.HasInfo("added");
      PluginFilter tested = original.Or(added);

      Assert.AreEqual(PluginFilter.FilterOperation.Or, tested.Operation);
      Assert.IsNull(tested.OperationData);
      Assert.AreEqual(2, tested.SubFilters.Length);
      Assert.IsTrue(tested.SubFilters.Contains(original));
      Assert.IsTrue(tested.SubFilters.Contains(added));
    }
    #endregion

    #region IsTypeOf
    [TestMethod]
    public void IsTypeOfShouldAndCombineWithNewIsTypeOfFilter()
    {
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      Type type = typeof(UnitTest_PluginFilter);
      PluginFilter tested = original.IsTypeOf(type);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.Operation);
      Assert.AreEqual(2, tested.SubFilters.Length);
      Assert.IsTrue(tested.SubFilters.Contains(original));
      Assert.IsTrue(tested.SubFilters.Any(x =>
             x.Operation == PluginFilter.FilterOperation.Or
          && x.SubFilters.Length == 2
          && x.SubFilters.Any(y => y.Operation == PluginFilter.FilterOperation.DerivesFrom && y.OperationData == type.AssemblyQualifiedName)
          && x.SubFilters.Any(y => y.Operation == PluginFilter.FilterOperation.Implements && y.OperationData == type.AssemblyQualifiedName)));
    }

    [TestMethod]
    public void IsTypeOfShouldNotAcceptNullType()
    {
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.IsTypeOf((Type)null));
    }

    [TestMethod]
    public void IsTypeOfShouldNotAcceptNullString()
    {
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.IsTypeOf((String)null));
    }
    #endregion

    #region Implements
    [TestMethod]
    public void ImplementsShouldAndCombineWithNewIsTypeOfFilter()
    {
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      Type type = typeof(UnitTest_PluginFilter);
      PluginFilter tested = original.Implements(type);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.Operation);
      Assert.AreEqual(2, tested.SubFilters.Length);
      Assert.IsTrue(tested.SubFilters.Contains(original));
      Assert.IsTrue(tested.SubFilters.Any(x => x.Operation == PluginFilter.FilterOperation.Implements && x.OperationData == type.AssemblyQualifiedName));
    }

    [TestMethod]
    public void ImplementsShouldNotAcceptNullType()
    {
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.Implements((Type)null));
    }

    [TestMethod]
    public void ImplementsShouldNotAcceptNullString()
    {
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.Implements((String)null));
    }
    #endregion

    #region DerivesFrom
    [TestMethod]
    public void DerivesFromShouldAndCombineWithNewIsTypeOfFilter()
    {
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      Type type = typeof(UnitTest_PluginFilter);
      PluginFilter tested = original.DerivesFrom(type);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.Operation);
      Assert.AreEqual(2, tested.SubFilters.Length);
      Assert.IsTrue(tested.SubFilters.Contains(original));
      Assert.IsTrue(tested.SubFilters.Any(x => x.Operation == PluginFilter.FilterOperation.DerivesFrom && x.OperationData == type.AssemblyQualifiedName));
    }

    [TestMethod]
    public void DerivesFromShouldNotAcceptNullType()
    {
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.DerivesFrom((Type)null));
    }

    [TestMethod]
    public void DerivesFromShouldNotAcceptNullString()
    {
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.DerivesFrom((String)null));
    }
    #endregion

    #region IsNamed
    [TestMethod]
    public void IsNamedShouldAndCombineWithNewIsTypeOfFilter()
    {
      var name = "name";
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      PluginFilter tested = original.IsNamed(name);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.Operation);
      Assert.AreEqual(2, tested.SubFilters.Length);
      Assert.IsTrue(tested.SubFilters.Contains(original));
      Assert.IsTrue(tested.SubFilters.Any(x => x.Operation == PluginFilter.FilterOperation.IsNamed && x.OperationData == name));
    }

    [TestMethod]
    public void IsNamedShouldNotAcceptNull()
    {
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.IsNamed((String)null));
    }
    #endregion

    #region HasInfo
    [TestMethod]
    public void HasInfoShouldAndCombineWithNewIsTypeOfFilter()
    {
      var name = "name";
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      PluginFilter tested = original.HasInfo(name);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.Operation);
      Assert.AreEqual(2, tested.SubFilters.Length);
      Assert.IsTrue(tested.SubFilters.Contains(original));
      Assert.IsTrue(tested.SubFilters.Any(x => x.Operation == PluginFilter.FilterOperation.HasInfo && x.OperationData == name));
    }

    [TestMethod]
    public void HasInfoShouldNotAcceptNull()
    {
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.HasInfo((String)null));
    }
    #endregion

    #region HasInfoValue
    [TestMethod]
    public void HasInfoValueShouldAndCombineWithNewIsTypeOfFilter()
    {
      var key = "key";
      var value = "value";
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      PluginFilter tested = original.HasInfoValue(key, value);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.Operation);
      Assert.AreEqual(2, tested.SubFilters.Length);
      Assert.IsTrue(tested.SubFilters.Contains(original));
      Assert.IsTrue(tested.SubFilters.Any(x => x.Operation == PluginFilter.FilterOperation.InfoValue && x.OperationData == key + "=" + value));
    }

    [TestMethod]
    public void HasInfoValueShouldNotAcceptNullKey()
    {
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.HasInfoValue(null, "value"));
    }

    [TestMethod]
    public void HasInfoValueShouldNotAcceptNullValue()
    {
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.HasInfoValue("key", null));
    }
    #endregion

    #region HasVersion
    [TestMethod]
    public void HasVersionShouldAndCombineWithNewIsTypeOfFilter()
    {
      var version = "1.0";
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      PluginFilter tested = original.HasVersion(version);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.Operation);
      Assert.AreEqual(3, tested.SubFilters.Length);
      Assert.IsTrue(tested.SubFilters.Contains(original));
      Assert.IsTrue(tested.SubFilters.Any(y => y.Operation == PluginFilter.FilterOperation.MinVersion && y.OperationData == version));
      Assert.IsTrue(tested.SubFilters.Any(y => y.Operation == PluginFilter.FilterOperation.MaxVersion && y.OperationData == version));
    }

    [TestMethod]
    public void HasVersionShouldNotAcceptNullString()
    {
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.HasVersion((String)null));
    }
    #endregion

    #region HasMinVersion
    [TestMethod]
    public void HasMinVersionShouldAndCombineWithNewIsTypeOfFilter()
    {
      var version = "1.0";
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      PluginFilter tested = original.HasMinVersion(version);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.Operation);
      Assert.AreEqual(2, tested.SubFilters.Length);
      Assert.IsTrue(tested.SubFilters.Contains(original));
      Assert.IsTrue(tested.SubFilters.Any(y => y.Operation == PluginFilter.FilterOperation.MinVersion && y.OperationData == version));
    }

    [TestMethod]
    public void HasMinVersionShouldNotAcceptNullString()
    {
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.HasMinVersion((String)null));
    }
    #endregion

    #region HasMaxVersion
    [TestMethod]
    public void HasMaxVersionShouldAndCombineWithNewIsTypeOfFilter()
    {
      var version = "1.0";
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      PluginFilter tested = original.HasMaxVersion(version);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.Operation);
      Assert.AreEqual(2, tested.SubFilters.Length);
      Assert.IsTrue(tested.SubFilters.Contains(original));
      Assert.IsTrue(tested.SubFilters.Any(y => y.Operation == PluginFilter.FilterOperation.MaxVersion && y.OperationData == version));
    }

    [TestMethod]
    public void HasMaxVersionShouldNotAcceptNullString()
    {
      PluginFilter original = PluginFilter.Create.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.HasMaxVersion((String)null));
    }
    #endregion

    #region Filter

    [TestMethod]
    public void FilterShouldNotAcceptNull()
    {
      DoAssert.Throws<ArgumentNullException>(() => PluginFilter.Create.IsNamed("name").Filter(null));
    }

    [TestMethod]
    public void FilterShouldApplyImplements()
    {
      PluginFilter tested = PluginFilter.Create.Implements(typeof(IMockPluginInterface1));
      PluginDescriptor[] filtered = tested.Filter(this.descriptors).ToArray();

      Assert.AreEqual(2, filtered.Length);
      Assert.IsTrue(filtered.Contains(plugin1));
      Assert.IsTrue(filtered.Contains(plugin3));
    }

    [TestMethod]
    public void FilterShouldApplyDerivesFrom()
    {
      PluginFilter tested = PluginFilter.Create.DerivesFrom(typeof(MockPluginBase));
      PluginDescriptor[] filtered = tested.Filter(this.descriptors).ToArray();

      Assert.AreEqual(1, filtered.Length);
      Assert.AreSame(this.plugin1, filtered.First());
    }

    [TestMethod]
    public void FilterShouldApplyIsNamed()
    {
      PluginFilter tested = PluginFilter.Create.IsNamed("MockPlugin1");
      PluginDescriptor[] filtered = tested.Filter(this.descriptors).ToArray();

      Assert.AreEqual(1, filtered.Length);
      Assert.AreSame(this.plugin1, filtered.First());
    }

    [TestMethod]
    public void FilterShouldApplyHasInfo()
    {
      PluginFilter tested = PluginFilter.Create.HasInfo("validkey");
      PluginDescriptor[] filtered = tested.Filter(this.descriptors).ToArray();

      Assert.AreEqual(1, filtered.Length);
      Assert.AreSame(this.plugin1, filtered.First());
    }

    [TestMethod]
    public void FilterShouldApplyHasInfoValue()
    {
      PluginFilter tested = PluginFilter.Create.HasInfoValue("validkey", "validvalue");
      PluginDescriptor[] filtered = tested.Filter(this.descriptors).ToArray();

      Assert.AreEqual(1, filtered.Length);
      Assert.AreSame(this.plugin1, filtered.First());
    }

    [TestMethod]
    public void FilterShouldApplyHasMinVersion()
    {
      PluginFilter tested = PluginFilter.Create.HasMinVersion("55.66");
      PluginDescriptor[] filtered = tested.Filter(this.descriptors).ToArray();

      Assert.AreEqual(2, filtered.Length);
      Assert.IsTrue(filtered.Contains(plugin1));
      Assert.IsTrue(filtered.Contains(plugin3));
    }

    [TestMethod]
    public void FilterShouldApplyHasMaxVersion()
    {
      PluginFilter tested = PluginFilter.Create.HasMaxVersion("55.66");
      PluginDescriptor[] filtered = tested.Filter(this.descriptors).ToArray();

      Assert.AreEqual(2, filtered.Length);
      Assert.IsTrue(filtered.Contains(plugin1));
      Assert.IsTrue(filtered.Contains(plugin2));
    }

    [TestMethod]
    public void FilterShouldApplyAndOperation()
    {
      PluginFilter tested = PluginFilter.Create.HasMaxVersion("55.66").And(PluginFilter.Create.HasMinVersion("55.66"));
      PluginDescriptor[] filtered = tested.Filter(this.descriptors).ToArray();

      Assert.AreEqual(1, filtered.Length);
      Assert.AreSame(plugin1, filtered.First());
    }

    [TestMethod]
    public void FilterShouldApplyOrOperation()
    {
      PluginFilter tested = PluginFilter.Create.HasMaxVersion("55.66").Or(PluginFilter.Create.HasMinVersion("55.66"));
      PluginDescriptor[] filtered = tested.Filter(this.descriptors).ToArray();

      Assert.AreEqual(3, filtered.Length);
      Assert.IsTrue(filtered.Contains(plugin1));
      Assert.IsTrue(filtered.Contains(plugin2));
      Assert.IsTrue(filtered.Contains(plugin3));
    }

    [TestMethod]
    public void FilterShouldThrowOnUnimplementedOperation()
    {
      DoAssert.Throws<NotImplementedException>(() =>
        new PluginFilter((PluginFilter.FilterOperation)9999, "kalle").Filter(this.descriptors));
    }

    #endregion

    #region ApplyMaxVersionFilter
    [TestMethod]
    public void ApplyMaxVersionFilterRequiresArgument()
    {
      DoAssert.Throws<ArgumentNullException>(() => PluginFilter.Create.HasMaxVersion("1.0").ApplyMaxVersionFilter(null));
    }
    #endregion

    #region ApplyMinVersionFilter
    [TestMethod]
    public void ApplyMinVersionFilterRequiresArgument()
    {
      DoAssert.Throws<ArgumentNullException>(() => PluginFilter.Create.HasMinVersion("1.0").ApplyMinVersionFilter(null));
    }
    #endregion

    #region ApplyHasInfoFilter
    [TestMethod]
    public void ApplyHasInfoFilterRequiresArgument()
    {
      DoAssert.Throws<ArgumentNullException>(() => PluginFilter.Create.HasInfo("key").ApplyHasInfoFilter(null));
    }
    #endregion

    #region ApplyInfoValueFilter
    [TestMethod]
    public void ApplyInfoValueFilterRequiresArgument()
    {
      DoAssert.Throws<ArgumentNullException>(() => PluginFilter.Create.HasInfoValue("key", "value").ApplyInfoValueFilter(null));
    }
    #endregion

    #region ApplyIsNamedFilter
    [TestMethod]
    public void ApplyIsNamedFilterRequiresArgument()
    {
      DoAssert.Throws<ArgumentNullException>(() => PluginFilter.Create.IsNamed("name").ApplyIsNamedFilter(null));
    }
    #endregion

    #region ApplyDerivesFromFilter
    [TestMethod]
    public void ApplyDerivesFromFilterRequiresArgument()
    {
      DoAssert.Throws<ArgumentNullException>(() => PluginFilter.Create.DerivesFrom(typeof(MockPluginBase)).ApplyDerivesFromFilter(null));
    }
    #endregion

    #region ApplyImplementsFilter
    [TestMethod]
    public void ApplyImplementsFilterRequiresArgument()
    {
      DoAssert.Throws<ArgumentNullException>(() => PluginFilter.Create.Implements(typeof(IMockPluginInterface1)).ApplyImplementsFilter(null));
    }
    #endregion

    #region ApplyAndFilter
    [TestMethod]
    public void ApplyAndFilterRequiresArgument()
    {
      DoAssert.Throws<ArgumentNullException>(() => PluginFilter.Create.IsNamed("name").HasInfo("key").ApplyAndFilter(null));
    }
    #endregion

    #region ApplyOrFilter
    [TestMethod]
    public void ApplyOrFilterRequiresArgument()
    {
      DoAssert.Throws<ArgumentNullException>(() => PluginFilter.Create.IsNamed("name").Or(PluginFilter.Create.HasInfo("key")).ApplyOrFilter(null));
    }
    #endregion

    #region ToString
    [TestMethod]
    public void ToStringUnaryOperatorExpectedValue()
    {
      PluginFilter.FilterOperation[] binaryOperators = new PluginFilter.FilterOperation[] {
        PluginFilter.FilterOperation.And,
        PluginFilter.FilterOperation.Or,
      };

      var unaryOperators = Enum.GetValues(typeof(PluginFilter.FilterOperation)).OfType<PluginFilter.FilterOperation>().Except(binaryOperators);

      foreach (var op in unaryOperators)
      {
        PluginFilter tested = new PluginFilter(op, "opdata", null);
        Assert.AreEqual(op.ToString() + "(opdata)", tested.ToString());
      }
    }

    [TestMethod]
    public void ToStringAndOperatorExpectedValue()
    {
      var sub1 = PluginFilter.Create.IsNamed("sub1");
      var sub2 = PluginFilter.Create.IsNamed("sub2");
      string expected = "(" + sub1.ToString() + " & " + sub2.ToString() + ")";
      PluginFilter tested = sub1.And(sub2);
      Assert.AreEqual(expected, tested.ToString());
    }

    [TestMethod]
    public void ToStringOrOperatorExpectedValue()
    {
      var sub1 = PluginFilter.Create.IsNamed("sub1");
      var sub2 = PluginFilter.Create.IsNamed("sub2");
      string expected = "(" + sub1.ToString() + " | " + sub2.ToString() + ")";
      PluginFilter tested = sub1.Or(sub2);
      Assert.AreEqual(expected, tested.ToString());
    }
    #endregion

    #region Equals
    [TestMethod]
    public void EqualsReturnsFalseWhenComparingAgainstNull()
    {
      PluginFilter left = PluginFilter.Create.IsNamed("name");
      Assert.IsFalse(left.Equals(null));
    }

    [TestMethod]
    public void EqualsReturnsFalseForDifferentOperation()
    {
      PluginFilter left = PluginFilter.Create.IsNamed("name");
      PluginFilter right = PluginFilter.Create.HasInfo("name");
      Assert.IsFalse(left.Equals(right));
    }

    [TestMethod]
    public void EqualsReturnsFalseForDifferentOperationData()
    {
      PluginFilter left = PluginFilter.Create.IsNamed("name");
      PluginFilter right = PluginFilter.Create.IsNamed("othername");
      Assert.IsFalse(left.Equals(right));
    }

    [TestMethod]
    public void EqualsReturnFalseForDifferentSubFilter()
    {
      PluginFilter left = PluginFilter.Create.IsNamed("name").Implements("type");
      PluginFilter right = PluginFilter.Create.IsNamed("name").DerivesFrom("type");
      Assert.IsFalse(left.Equals(right));
    }
    #endregion

    #region Serialization
    [TestMethod]
    public void CanSerializeDeserialize()
    {
      PluginFilter toSerialize = PluginFilter.Create.IsNamed("some name").Implements(typeof(string)).Or(PluginFilter.Create.DerivesFrom(typeof(int)).IsNamed("a name").HasVersion("1.0"));
      var knownTypes = new Type[] { typeof(PluginFilter.FilterOperation), typeof(PluginFilter[]) };
      PluginFilter deserialized;

      using (var memstream = new MemoryStream())
      {
        XmlTextWriter writer = new XmlTextWriter(memstream, Encoding.UTF8);
        var serializer = new DataContractSerializer(toSerialize.GetType(), knownTypes);
        serializer.WriteObject(writer, toSerialize);
        writer.Flush();

        memstream.Seek(0, SeekOrigin.Begin);
        XmlTextReader reader = new XmlTextReader(memstream);
        deserialized = serializer.ReadObject(reader) as PluginFilter;
      }

      Assert.IsTrue(deserialized.Equals(toSerialize));
    }

    [TestMethod]
    public void SerializationRequiresSerializationInfo()
    {
      ISerializable tested = PluginFilter.Create.Implements(typeof(Stream));
      DoAssert.Throws<ArgumentNullException>(() => tested.GetObjectData(null, new StreamingContext()));
    }

    [TestMethod]
    public void DeserializationRequiresSerializationInfo()
    {      
      DoAssert.Throws<ArgumentNullException>(() => new PluginFilter(null, new StreamingContext()));
    }
    #endregion

  }
}
