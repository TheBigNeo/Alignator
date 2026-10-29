using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace TBN.Alignator.Tests.CodeFormatter.CSharp.AlignArguments;

/// <summary>
///     Guards <see cref="MixedContextsFormattingTest.TestAllSettingsAtOnce"/> against gaps in its settings
///     coverage: every key declared on <see cref="AlignatorSettingsKey"/> must be set on the test method via
///     <c>[TestSetting]</c> - a newly added key without a matching TestSetting fails this test, so the
///     showcase always exercises the full settings surface.
/// </summary>
[TestFixture]
public class AllSettingsAtOnceCoverageTest
{
    [Test]
    public void TestAllSettingsAtOnce_SetsEveryAlignatorSettingsKey()
    {
        var expectedKeys = typeof(AlignatorSettingsKey)
           .GetFields(BindingFlags.Public | BindingFlags.Instance)
           .Select(field => field.Name)
           .OrderBy(name => name)
           .ToList();

        var testMethod = typeof(MixedContextsFormattingTest)
           .GetMethod(nameof(MixedContextsFormattingTest.TestAllSettingsAtOnce),
                BindingFlags.Public | BindingFlags.Instance);

        Assert.That(testMethod, Is.Not.Null,
            "MixedContextsFormattingTest.TestAllSettingsAtOnce not found - the showcase test was renamed or removed");

        var usedKeys = testMethod!
           .GetCustomAttributesData()
           .Select(data => data.ConstructorArguments)
           .Where(args => args.Count >= 2 && args[0].Value is Type type && type == typeof(AlignatorSettingsKey))
           .Select(args => (string)args[1].Value!)
           .OrderBy(name => name)
           .ToList();

        Assert.That(usedKeys, Is.EqualTo(expectedKeys),
            "TestAllSettingsAtOnce must set every AlignatorSettingsKey exactly once. " +
            $"Missing: [{string.Join(", ", expectedKeys.Except(usedKeys))}]. " +
            $"Unexpected or duplicated: [{string.Join(", ", usedKeys.Except(expectedKeys))}]");
    }
}
