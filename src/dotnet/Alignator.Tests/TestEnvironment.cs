using System;
using System.IO;
using System.Threading;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.Application.Environment;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using JetBrains.TestFramework.Utils;
using NUnit.Framework;

// @formatter:empty_block_style together_same_line

[assembly: Apartment(ApartmentState.STA)]

namespace TBN.Alignator.Tests;

[ZoneDefinition]
public class AlignatorTestEnvironmentZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>, IRequire<IAlignatorZone>;

[ZoneMarker]
public class ZoneMarker : IRequire<ICodeEditingZone>, IRequire<ILanguageCSharpZone>, IRequire<AlignatorTestEnvironmentZone> { }
// public class ZoneMarker : IRequire<IAlignatorZone>;

[ZoneActivator]
public class ZoneActivator : IActivate<PsiFeatureTestZone>, IActivate<IAlignatorZone> { }

[SetUpFixture]
public class AlignatorTestsAssembly : ExtensionTestEnvironmentAssembly<AlignatorTestEnvironmentZone>
{
    static AlignatorTestsAssembly()
    {
        SetJetTestPackagesDir();
    }

    private static void SetJetTestPackagesDir()
    {
        if (Environment.GetEnvironmentVariable("JET_TEST_PACKAGES_DIR") != null)
        {
            return;
        }

        var packages = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "JetTestPackages");

        if (Directory.Exists(packages) is false)
        {
            TestUtil.SetHomeDir(typeof(AlignatorTestsAssembly).Assembly);
            var testData = TestUtil.GetTestDataPathBase(typeof(AlignatorTestsAssembly).Assembly);
            packages = testData.Parent.Combine("JetTestPackages").FullPath;
        }

        if (Directory.Exists(packages) is false)
        {
            Directory.CreateDirectory(packages);
        }

        Environment.SetEnvironmentVariable("JET_TEST_PACKAGES_DIR", packages, EnvironmentVariableTarget.Process);
    }
}