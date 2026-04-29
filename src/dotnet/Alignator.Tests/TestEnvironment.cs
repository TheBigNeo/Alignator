using System.Threading;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.Feature.Services;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;

[assembly: Apartment(ApartmentState.STA)]

namespace TBN.Alignator.Tests;

[ZoneDefinition]
public class AlignatorTestEnvironmentZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>, IRequire<IAlignatorZone> { }

[ZoneMarker]
public class ZoneMarker : IRequire<ICodeEditingZone>, IRequire<ILanguageCSharpZone>, IRequire<AlignatorTestEnvironmentZone> { }

[SetUpFixture]
public class AlignatorTestsAssembly : ExtensionTestEnvironmentAssembly<AlignatorTestEnvironmentZone> { }