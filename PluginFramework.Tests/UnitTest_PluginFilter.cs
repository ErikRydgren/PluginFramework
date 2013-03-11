using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        QualifiedName = typeof(MockPlugin1).AssemblyQualifiedName,
        Name = "MockPlugin1",
        Version = new PluginVersion("55.66")
      };
      plugin1.Derives.Add(typeof(MockPluginBase).AssemblyQualifiedName);
      plugin1.Derives.Add(typeof(MarshalByRefObject).AssemblyQualifiedName);
      plugin1.Derives.Add(typeof(object).AssemblyQualifiedName);
      plugin1.Interfaces.Add(typeof(IMockPluginInterface1).AssemblyQualifiedName);
      plugin1.InfoValues.Add("validkey", "validvalue");
      plugin1.Settings.Add(new PluginSettingDescriptor() { Name = "SettingName", Required = false, SettingType = typeof(int).AssemblyQualifiedName });

      plugin2 = new PluginDescriptor()
      {
        QualifiedName = typeof(MockPlugin2).AssemblyQualifiedName,
        Name = "MockPlugin2",
        Version = new PluginVersion("33.88")
      };
      plugin2.Derives.Add(typeof(MarshalByRefObject).AssemblyQualifiedName);
      plugin2.Derives.Add(typeof(object).AssemblyQualifiedName);
      plugin2.Interfaces.Add(typeof(IMockPluginInterface2).AssemblyQualifiedName);
      plugin2.InfoValues.Add("invalidkey", "invalidvalue");
      plugin2.Settings.Add(new PluginSettingDescriptor() { Name = "InvalidSettingName", Required = false, SettingType = typeof(int).AssemblyQualifiedName });

      plugin3 = new PluginDescriptor()
      {
        QualifiedName = typeof(MockPlugin3).AssemblyQualifiedName,
        Name = "MockPlugin3",
        Version = new PluginVersion("77.88")
      };
      plugin3.Derives.Add(typeof(MarshalByRefObject).AssemblyQualifiedName);
      plugin3.Derives.Add(typeof(object).AssemblyQualifiedName);
      plugin3.Interfaces.Add(typeof(IMockPluginInterface1).AssemblyQualifiedName);
      plugin3.InfoValues.Add("invalidkey", "invalidvalue");
      plugin3.Settings.Add(new PluginSettingDescriptor() { Name = "InvalidSettingName", Required = false, SettingType = typeof(int).AssemblyQualifiedName });

      descriptors.Add(plugin2);
      descriptors.Add(plugin1);
      descriptors.Add(plugin3);
    }

    #region Constructing

    #region IsTypeOf
    [TestMethod]
    public void ConstructingIsTypeOfStatic()
    {
      Type type = typeof(UnitTest_PluginFilter);
      PluginFilter tested = Plugin.IsTypeOf(type);

      Assert.IsNotNull(tested);
      Assert.AreEqual(PluginFilter.FilterOperation.Or, tested.operation);
      Assert.AreEqual(2, tested.subFilters.Length);

      var sub = tested.subFilters;
      var filterImplements = sub.FirstOrDefault(x => x.operation == PluginFilter.FilterOperation.Implements);
      var filterDerivesFrom = sub.FirstOrDefault(x => x.operation == PluginFilter.FilterOperation.DerivesFrom);

      Assert.IsNotNull(filterImplements);
      Assert.IsNotNull(filterDerivesFrom);

      Assert.AreEqual(type.AssemblyQualifiedName, filterImplements.operationData);
      Assert.AreEqual(type.AssemblyQualifiedName, filterDerivesFrom.operationData);
    }

    [TestMethod]
    public void ConstructionIsTypeOfStaticRejectNullType()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.IsTypeOf((Type)null));
    }

    [TestMethod]
    public void ConstructionIsTypeOfStaticRejectNullString()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.IsTypeOf((string)null));
    }
    #endregion

    #region Implements
    [TestMethod]
    public void ConstructionImplementsStatic()
    {
      Type type = typeof(UnitTest_PluginFilter);
      PluginFilter tested = Plugin.Implements(type);

      Assert.IsNotNull(tested);
      Assert.AreEqual(PluginFilter.FilterOperation.Implements, tested.operation);
      Assert.AreEqual(type.AssemblyQualifiedName, tested.operationData);
      Assert.IsNull(tested.subFilters);
    }

    [TestMethod]
    public void ConstructionImplementsStaticRejectNullType()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.Implements((Type)null));
    }

    [TestMethod]
    public void ConstructionImplementsStaticRejectNullString()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.Implements((string)null));
    }
    #endregion

    #region DerivesFrom
    [TestMethod]
    public void ConstructionDerivesFromStatic()
    {
      Type type = typeof(UnitTest_PluginFilter);
      PluginFilter tested = Plugin.DerivesFrom(type);

      Assert.IsNotNull(tested);
      Assert.AreEqual(PluginFilter.FilterOperation.DerivesFrom, tested.operation);
      Assert.AreEqual(type.AssemblyQualifiedName, tested.operationData);
      Assert.IsNull(tested.subFilters);
    }

    [TestMethod]
    public void ConstructionDerivesFromStaticRejectNullType()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.DerivesFrom((Type)null));
    }

    [TestMethod]
    public void ConstructionDerivesFromStaticRejectNullString()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.DerivesFrom((string)null));
    }
    #endregion

    #region IsNamed
    [TestMethod]
    public void ConstructionIsNamedFromStatic()
    {
      var name = "AName";
      PluginFilter tested = Plugin.IsNamed(name);

      Assert.IsNotNull(tested);
      Assert.AreEqual(PluginFilter.FilterOperation.IsNamed, tested.operation);
      Assert.AreEqual(name, tested.operationData);
      Assert.IsNull(tested.subFilters);
    }

    [TestMethod]
    public void ConstructionIsNamedFromStaticRejectNull()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.IsNamed(null));
    }
    #endregion

    #region HasInfo
    [TestMethod]
    public void ConstructionHasInfoFromStatic()
    {
      var name = "AName";
      PluginFilter tested = Plugin.HasInfo(name);

      Assert.IsNotNull(tested);
      Assert.AreEqual(PluginFilter.FilterOperation.HasInfo, tested.operation);
      Assert.AreEqual(name, tested.operationData);
      Assert.IsNull(tested.subFilters);
    }

    [TestMethod]
    public void ConstructionHasInfoFromStaticRejectNull()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.HasInfo(null));
    }
    #endregion

    #region HasInfoValue
    [TestMethod]
    public void ConstructionHasInfoValueFromStatic()
    {
      var key = "AKey";
      var value = "AValue";
      PluginFilter tested = Plugin.HasInfoValue(key, value);

      Assert.IsNotNull(tested);
      Assert.AreEqual(PluginFilter.FilterOperation.InfoValue, tested.operation);
      Assert.AreEqual(key + "=" + value, tested.operationData);
      Assert.IsNull(tested.subFilters);
    }

    [TestMethod]
    public void ConstructionHasInfoValueFromStaticRequiresKey()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.HasInfoValue(null, "Value"));
    }

    [TestMethod]
    public void ConstructionHasInfoValueFromStaticRequiresValue()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.HasInfoValue("Key", null));
    }
    #endregion

    #region HasVersion
    [TestMethod]
    public void ConstructingHasVersionStatic()
    {
      var version = "1.0";
      PluginFilter tested = Plugin.HasVersion(version);

      Assert.IsNotNull(tested);
      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.operation);
      Assert.AreEqual(2, tested.subFilters.Length);

      var sub = tested.subFilters;
      var filterImplements = sub.FirstOrDefault(x => x.operation == PluginFilter.FilterOperation.MinVersion);
      var filterDerivesFrom = sub.FirstOrDefault(x => x.operation == PluginFilter.FilterOperation.MaxVersion);

      Assert.IsNotNull(filterImplements);
      Assert.IsNotNull(filterDerivesFrom);

      Assert.AreEqual(version, filterImplements.operationData);
      Assert.AreEqual(version, filterDerivesFrom.operationData);
    }

    [TestMethod]
    public void ConstructionHasVersionStaticRejectNullString()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.HasVersion((string)null));
    }
    #endregion

    #region HasMinVersion
    [TestMethod]
    public void ConstructingHasMinVersionStatic()
    {
      var version = "1.0";

      PluginFilter tested = Plugin.HasMinVersion(version);
      Assert.IsNotNull(tested);
      Assert.AreEqual(PluginFilter.FilterOperation.MinVersion, tested.operation);
      Assert.AreEqual(version, tested.operationData);
      Assert.IsNull(tested.subFilters);
    }

    [TestMethod]
    public void ConstructingHasMinVersionStaticRejectNullString()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.HasMinVersion((string)null));
    }
    #endregion

    #region HasMaxVersion
    [TestMethod]
    public void ConstructingHasMaxVersionStatic()
    {
      var version = "1.0";

      PluginFilter tested = Plugin.HasMaxVersion(version);
      Assert.IsNotNull(tested);
      Assert.AreEqual(PluginFilter.FilterOperation.MaxVersion, tested.operation);
      Assert.AreEqual(version, tested.operationData);
      Assert.IsNull(tested.subFilters);
    }

    [TestMethod]
    public void ConstructingHasMaxVersionStaticRejectNullString()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.HasMaxVersion((string)null));
    }
    #endregion

    #endregion

    #region Combine
    [TestMethod]
    public void CombineBecomesSubFilters()
    {
      var left = Plugin.IsNamed("left");
      var right = Plugin.IsNamed("right");

      PluginFilter tested = PluginFilter.Combine(PluginFilter.FilterOperation.And, left, right);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.operation);
      Assert.IsNull(tested.operationData);
      Assert.IsNotNull(tested.subFilters);
      Assert.AreEqual(2, tested.subFilters.Length);
      Assert.AreSame(left, tested.subFilters[0]);
      Assert.AreSame(right, tested.subFilters[1]);
    }

    [TestMethod]
    public void CombineOnlyAcceptBooleanOperators()
    {
      PluginFilter.FilterOperation[] valid = new PluginFilter.FilterOperation[] {
        PluginFilter.FilterOperation.And,
        PluginFilter.FilterOperation.Or,
      };

      var invalid = Enum.GetValues(typeof(PluginFilter.FilterOperation)).OfType<PluginFilter.FilterOperation>().Except(valid);

      var left = Plugin.IsNamed("left");
      var right = Plugin.IsNamed("right");

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
      DoAssert.Throws<ArgumentNullException>(() => PluginFilter.Combine(PluginFilter.FilterOperation.And, null, Plugin.IsNamed("right")));
    }

    [TestMethod]
    public void CombineRequiresRightArgument()
    {
      DoAssert.Throws<ArgumentNullException>(() => PluginFilter.Combine(PluginFilter.FilterOperation.And, Plugin.IsNamed("left"), null));
    }

    [TestMethod]
    public void CombineShouldMergeIfArgsAreSameOperator()
    {
      var op = PluginFilter.FilterOperation.Or;
      PluginFilter left1 = Plugin.IsNamed("left1");
      PluginFilter right1 = Plugin.IsNamed("right1");
      PluginFilter combined1 = PluginFilter.Combine(op, left1, right1);

      PluginFilter left2 = Plugin.IsNamed("left2");
      PluginFilter right2 = Plugin.IsNamed("right2");
      PluginFilter combined2 = PluginFilter.Combine(op, left2, right2);

      PluginFilter tested = PluginFilter.Combine(op, combined1, combined2);

      Assert.AreEqual(op, tested.operation);
      Assert.AreEqual(4, tested.subFilters.Length);
      Assert.IsTrue(tested.subFilters.Contains(left1));
      Assert.IsTrue(tested.subFilters.Contains(right1));
      Assert.IsTrue(tested.subFilters.Contains(left2));
      Assert.IsTrue(tested.subFilters.Contains(right2));
    }

    [TestMethod]
    public void CombineShouldMergeIfLeftIsSameOperator()
    {
      var op = PluginFilter.FilterOperation.Or;
      PluginFilter left1 = Plugin.IsNamed("left1");
      PluginFilter right1 = Plugin.IsNamed("right1");
      PluginFilter combined1 = PluginFilter.Combine(op, left1, right1);
      PluginFilter right2 = Plugin.IsNamed("right2");

      PluginFilter tested = PluginFilter.Combine(op, combined1, right2);
      Assert.AreEqual(op, tested.operation);
      Assert.AreEqual(3, tested.subFilters.Length);
      Assert.IsTrue(tested.subFilters.Contains(left1));
      Assert.IsTrue(tested.subFilters.Contains(right1));
      Assert.IsTrue(tested.subFilters.Contains(right2));
    }

    [TestMethod]
    public void CombineShouldMergeIfRightIsSameOperator()
    {
      var op = PluginFilter.FilterOperation.Or;
      PluginFilter left1 = Plugin.IsNamed("right2");

      PluginFilter left2 = Plugin.IsNamed("left1");
      PluginFilter right2 = Plugin.IsNamed("right1");
      PluginFilter combined2 = PluginFilter.Combine(op, left2, right2);

      PluginFilter tested = PluginFilter.Combine(op, left1, combined2);
      Assert.AreEqual(op, tested.operation);
      Assert.AreEqual(3, tested.subFilters.Length);
      Assert.IsTrue(tested.subFilters.Contains(left1));
      Assert.IsTrue(tested.subFilters.Contains(left2));
      Assert.IsTrue(tested.subFilters.Contains(right2));
    }
    #endregion

    #region And
    [TestMethod]
    public void AndRequiresArgument()
    {
      PluginFilter tested = Plugin.HasVersion("1.0");
      DoAssert.Throws<ArgumentNullException>(() => tested.And(null));
    }

    [TestMethod]
    public void AndCreatesAndOperatorWithSubFilters()
    {
      PluginFilter original = Plugin.IsNamed("original");
      PluginFilter added = Plugin.HasInfo("added");
      PluginFilter tested = original.And(added);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.operation);
      Assert.IsNull(tested.operationData);
      Assert.AreEqual(2, tested.subFilters.Length);
      Assert.IsTrue(tested.subFilters.Contains(original));
      Assert.IsTrue(tested.subFilters.Contains(added));
    }
    #endregion

    #region Or
    [TestMethod]
    public void OrRequiresArgument()
    {
      PluginFilter tested = Plugin.HasVersion("1.0");
      DoAssert.Throws<ArgumentNullException>(() => tested.Or(null));
    }

    [TestMethod]
    public void OrCreatesOrOperatorWithSubFilters()
    {
      PluginFilter original = Plugin.IsNamed("original");
      PluginFilter added = Plugin.HasInfo("added");
      PluginFilter tested = original.Or(added);

      Assert.AreEqual(PluginFilter.FilterOperation.Or, tested.operation);
      Assert.IsNull(tested.operationData);
      Assert.AreEqual(2, tested.subFilters.Length);
      Assert.IsTrue(tested.subFilters.Contains(original));
      Assert.IsTrue(tested.subFilters.Contains(added));
    }
    #endregion

    #region IsTypeOf
    [TestMethod]
    public void IsTypeOfShouldAndCombineWithNewIsTypeOfFilter()
    {
      PluginFilter original = Plugin.IsNamed("original");
      Type type = typeof(UnitTest_PluginFilter);
      PluginFilter tested = original.IsTypeOf(type);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.operation);
      Assert.AreEqual(2, tested.subFilters.Length);
      Assert.IsTrue(tested.subFilters.Contains(original));
      Assert.IsTrue(tested.subFilters.Any(x =>
             x.operation == PluginFilter.FilterOperation.Or
          && x.subFilters.Length == 2
          && x.subFilters.Any(y => y.operation == PluginFilter.FilterOperation.DerivesFrom && y.operationData == type.AssemblyQualifiedName)
          && x.subFilters.Any(y => y.operation == PluginFilter.FilterOperation.Implements && y.operationData == type.AssemblyQualifiedName)));
    }

    [TestMethod]
    public void IsTypeOfShouldNotAcceptNullType()
    {
      PluginFilter original = Plugin.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.IsTypeOf((Type)null));
    }

    [TestMethod]
    public void IsTypeOfShouldNotAcceptNullString()
    {
      PluginFilter original = Plugin.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.IsTypeOf((String)null));
    }
    #endregion

    #region Implements
    [TestMethod]
    public void ImplementsShouldAndCombineWithNewIsTypeOfFilter()
    {
      PluginFilter original = Plugin.IsNamed("original");
      Type type = typeof(UnitTest_PluginFilter);
      PluginFilter tested = original.Implements(type);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.operation);
      Assert.AreEqual(2, tested.subFilters.Length);
      Assert.IsTrue(tested.subFilters.Contains(original));
      Assert.IsTrue(tested.subFilters.Any(x => x.operation == PluginFilter.FilterOperation.Implements && x.operationData == type.AssemblyQualifiedName));
    }

    [TestMethod]
    public void ImplementsShouldNotAcceptNullType()
    {
      PluginFilter original = Plugin.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.Implements((Type)null));
    }

    [TestMethod]
    public void ImplementsShouldNotAcceptNullString()
    {
      PluginFilter original = Plugin.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.Implements((String)null));
    }
    #endregion

    #region DerivesFrom
    [TestMethod]
    public void DerivesFromShouldAndCombineWithNewIsTypeOfFilter()
    {
      PluginFilter original = Plugin.IsNamed("original");
      Type type = typeof(UnitTest_PluginFilter);
      PluginFilter tested = original.DerivesFrom(type);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.operation);
      Assert.AreEqual(2, tested.subFilters.Length);
      Assert.IsTrue(tested.subFilters.Contains(original));
      Assert.IsTrue(tested.subFilters.Any(x => x.operation == PluginFilter.FilterOperation.DerivesFrom && x.operationData == type.AssemblyQualifiedName));
    }

    [TestMethod]
    public void DerivesFromShouldNotAcceptNullType()
    {
      PluginFilter original = Plugin.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.DerivesFrom((Type)null));
    }

    [TestMethod]
    public void DerivesFromShouldNotAcceptNullString()
    {
      PluginFilter original = Plugin.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.DerivesFrom((String)null));
    }
    #endregion

    #region IsNamed
    [TestMethod]
    public void IsNamedShouldAndCombineWithNewIsTypeOfFilter()
    {
      var name = "name";
      PluginFilter original = Plugin.IsNamed("original");
      PluginFilter tested = original.IsNamed(name);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.operation);
      Assert.AreEqual(2, tested.subFilters.Length);
      Assert.IsTrue(tested.subFilters.Contains(original));
      Assert.IsTrue(tested.subFilters.Any(x => x.operation == PluginFilter.FilterOperation.IsNamed && x.operationData == name));
    }

    [TestMethod]
    public void IsNamedShouldNotAcceptNull()
    {
      PluginFilter original = Plugin.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.IsNamed((String)null));
    }
    #endregion

    #region HasInfo
    [TestMethod]
    public void HasInfoShouldAndCombineWithNewIsTypeOfFilter()
    {
      var name = "name";
      PluginFilter original = Plugin.IsNamed("original");
      PluginFilter tested = original.HasInfo(name);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.operation);
      Assert.AreEqual(2, tested.subFilters.Length);
      Assert.IsTrue(tested.subFilters.Contains(original));
      Assert.IsTrue(tested.subFilters.Any(x => x.operation == PluginFilter.FilterOperation.HasInfo && x.operationData == name));
    }

    [TestMethod]
    public void HasInfoShouldNotAcceptNull()
    {
      PluginFilter original = Plugin.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.HasInfo((String)null));
    }
    #endregion

    #region HasInfoValue
    [TestMethod]
    public void HasInfoValueShouldAndCombineWithNewIsTypeOfFilter()
    {
      var key = "key";
      var value = "value";
      PluginFilter original = Plugin.IsNamed("original");
      PluginFilter tested = original.HasInfoValue(key, value);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.operation);
      Assert.AreEqual(2, tested.subFilters.Length);
      Assert.IsTrue(tested.subFilters.Contains(original));
      Assert.IsTrue(tested.subFilters.Any(x => x.operation == PluginFilter.FilterOperation.InfoValue && x.operationData == key + "=" + value));
    }

    [TestMethod]
    public void HasInfoValueShouldNotAcceptNullKey()
    {
      PluginFilter original = Plugin.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.HasInfoValue(null, "value"));
    }

    [TestMethod]
    public void HasInfoValueShouldNotAcceptNullValue()
    {
      PluginFilter original = Plugin.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.HasInfoValue("key", null));
    }
    #endregion

    #region HasVersion
    [TestMethod]
    public void HasVersionShouldAndCombineWithNewIsTypeOfFilter()
    {
      var version = "1.0";
      PluginFilter original = Plugin.IsNamed("original");
      PluginFilter tested = original.HasVersion(version);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.operation);
      Assert.AreEqual(3, tested.subFilters.Length);
      Assert.IsTrue(tested.subFilters.Contains(original));
      Assert.IsTrue(tested.subFilters.Any(y => y.operation == PluginFilter.FilterOperation.MinVersion && y.operationData == version));
      Assert.IsTrue(tested.subFilters.Any(y => y.operation == PluginFilter.FilterOperation.MaxVersion && y.operationData == version));
    }

    [TestMethod]
    public void HasVersionShouldNotAcceptNullString()
    {
      PluginFilter original = Plugin.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.HasVersion((String)null));
    }
    #endregion

    #region HasMinVersion
    [TestMethod]
    public void HasMinVersionShouldAndCombineWithNewIsTypeOfFilter()
    {
      var version = "1.0";
      PluginFilter original = Plugin.IsNamed("original");
      PluginFilter tested = original.HasMinVersion(version);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.operation);
      Assert.AreEqual(2, tested.subFilters.Length);
      Assert.IsTrue(tested.subFilters.Contains(original));
      Assert.IsTrue(tested.subFilters.Any(y => y.operation == PluginFilter.FilterOperation.MinVersion && y.operationData == version));
    }

    [TestMethod]
    public void HasMinVersionShouldNotAcceptNullString()
    {
      PluginFilter original = Plugin.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.HasMinVersion((String)null));
    }
    #endregion

    #region HasMaxVersion
    [TestMethod]
    public void HasMaxVersionShouldAndCombineWithNewIsTypeOfFilter()
    {
      var version = "1.0";
      PluginFilter original = Plugin.IsNamed("original");
      PluginFilter tested = original.HasMaxVersion(version);

      Assert.AreEqual(PluginFilter.FilterOperation.And, tested.operation);
      Assert.AreEqual(2, tested.subFilters.Length);
      Assert.IsTrue(tested.subFilters.Contains(original));
      Assert.IsTrue(tested.subFilters.Any(y => y.operation == PluginFilter.FilterOperation.MaxVersion && y.operationData == version));
    }

    [TestMethod]
    public void HasMaxVersionShouldNotAcceptNullString()
    {
      PluginFilter original = Plugin.IsNamed("original");
      DoAssert.Throws<ArgumentNullException>(() => original.HasMaxVersion((String)null));
    }
    #endregion

    #region Filter

    [TestMethod]
    public void FilterShouldNotAcceptNull()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.IsNamed("name").Filter(null));
    }

    [TestMethod]
    public void FilterShouldApplyImplements()
    {
      PluginFilter tested = Plugin.Implements(typeof(IMockPluginInterface1));
      PluginDescriptor[] filtered = tested.Filter(this.descriptors).ToArray();

      Assert.AreEqual(2, filtered.Length);
      Assert.IsTrue(filtered.Contains(plugin1));
      Assert.IsTrue(filtered.Contains(plugin3));
    }

    [TestMethod]
    public void FilterShouldApplyDerivesFrom()
    {
      PluginFilter tested = Plugin.DerivesFrom(typeof(MockPluginBase));
      PluginDescriptor[] filtered = tested.Filter(this.descriptors).ToArray();

      Assert.AreEqual(1, filtered.Length);
      Assert.AreSame(this.plugin1, filtered.First());
    }

    [TestMethod]
    public void FilterShouldApplyIsNamed()
    {
      PluginFilter tested = Plugin.IsNamed("MockPlugin1");
      PluginDescriptor[] filtered = tested.Filter(this.descriptors).ToArray();

      Assert.AreEqual(1, filtered.Length);
      Assert.AreSame(this.plugin1, filtered.First());
    }

    [TestMethod]
    public void FilterShouldApplyHasInfo()
    {
      PluginFilter tested = Plugin.HasInfo("validkey");
      PluginDescriptor[] filtered = tested.Filter(this.descriptors).ToArray();

      Assert.AreEqual(1, filtered.Length);
      Assert.AreSame(this.plugin1, filtered.First());
    }

    [TestMethod]
    public void FilterShouldApplyHasInfoValue()
    {
      PluginFilter tested = Plugin.HasInfoValue("validkey", "validvalue");
      PluginDescriptor[] filtered = tested.Filter(this.descriptors).ToArray();

      Assert.AreEqual(1, filtered.Length);
      Assert.AreSame(this.plugin1, filtered.First());
    }

    [TestMethod]
    public void FilterShouldApplyHasMinVersion()
    {
      PluginFilter tested = Plugin.HasMinVersion("55.66");
      PluginDescriptor[] filtered = tested.Filter(this.descriptors).ToArray();

      Assert.AreEqual(2, filtered.Length);
      Assert.IsTrue(filtered.Contains(plugin1));
      Assert.IsTrue(filtered.Contains(plugin3));
    }

    [TestMethod]
    public void FilterShouldApplyHasMaxVersion()
    {
      PluginFilter tested = Plugin.HasMaxVersion("55.66");
      PluginDescriptor[] filtered = tested.Filter(this.descriptors).ToArray();

      Assert.AreEqual(2, filtered.Length);
      Assert.IsTrue(filtered.Contains(plugin1));
      Assert.IsTrue(filtered.Contains(plugin2));
    }

    [TestMethod]
    public void FilterShouldApplyAndOperation()
    {
      PluginFilter tested = Plugin.HasMaxVersion("55.66").And(Plugin.HasMinVersion("55.66"));
      PluginDescriptor[] filtered = tested.Filter(this.descriptors).ToArray();

      Assert.AreEqual(1, filtered.Length);
      Assert.AreSame(plugin1, filtered.First());
    }

    [TestMethod]
    public void FilterShouldApplyOrOperation()
    {
      PluginFilter tested = Plugin.HasMaxVersion("55.66").Or(Plugin.HasMinVersion("55.66"));
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
      DoAssert.Throws<ArgumentNullException>(() => Plugin.HasMaxVersion("1.0").ApplyMaxVersionFilter(null));
    }
    #endregion

    #region ApplyMinVersionFilter
    [TestMethod]
    public void ApplyMinVersionFilterRequiresArgument()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.HasMinVersion("1.0").ApplyMinVersionFilter(null));
    }
    #endregion

    #region ApplyHasInfoFilter
    [TestMethod]
    public void ApplyHasInfoFilterRequiresArgument()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.HasInfo("key").ApplyHasInfoFilter(null));
    }
    #endregion

    #region ApplyInfoValueFilter
    [TestMethod]
    public void ApplyInfoValueFilterRequiresArgument()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.HasInfoValue("key", "value").ApplyInfoValueFilter(null));
    }
    #endregion

    #region ApplyIsNamedFilter
    [TestMethod]
    public void ApplyIsNamedFilterRequiresArgument()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.IsNamed("name").ApplyIsNamedFilter(null));
    }
    #endregion

    #region ApplyDerivesFromFilter
    [TestMethod]
    public void ApplyDerivesFromFilterRequiresArgument()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.DerivesFrom(typeof(MockPluginBase)).ApplyDerivesFromFilter(null));
    }
    #endregion

    #region ApplyImplementsFilter
    [TestMethod]
    public void ApplyImplementsFilterRequiresArgument()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.Implements(typeof(IMockPluginInterface1)).ApplyImplementsFilter(null));
    }
    #endregion

    #region ApplyAndFilter
    [TestMethod]
    public void ApplyAndFilterRequiresArgument()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.IsNamed("name").HasInfo("key").ApplyAndFilter(null));
    }
    #endregion

    #region ApplyOrFilter
    [TestMethod]
    public void ApplyOrFilterRequiresArgument()
    {
      DoAssert.Throws<ArgumentNullException>(() => Plugin.IsNamed("name").Or(Plugin.HasInfo("key")).ApplyOrFilter(null));
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
      var sub1 = Plugin.IsNamed("sub1");
      var sub2 = Plugin.IsNamed("sub2");
      string expected = "(" + sub1.ToString() + " & " + sub2.ToString() + ")";
      PluginFilter tested = sub1.And(sub2);
      Assert.AreEqual(expected, tested.ToString());
    }

    [TestMethod]
    public void ToStringOrOperatorExpectedValue()
    {
      var sub1 = Plugin.IsNamed("sub1");
      var sub2 = Plugin.IsNamed("sub2");
      string expected = "(" + sub1.ToString() + " | " + sub2.ToString() + ")";
      PluginFilter tested = sub1.Or(sub2);
      Assert.AreEqual(expected, tested.ToString());
    }
    #endregion
  }
}
