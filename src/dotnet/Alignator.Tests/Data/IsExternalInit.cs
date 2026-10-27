// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices;

// Records (C# 9) on net472: the SDK test assemblies declare this type ambiguously
// (CS8356 in two referenced assemblies); a source definition takes precedence.
internal static class IsExternalInit { }
