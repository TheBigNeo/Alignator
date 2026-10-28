// Attribute stubs for the test data of this folder. They live in the test-data namespace so
// they take precedence over any equally named attribute from a using scope: the data files
// resolve 'Test', 'TestCase', ... to these stubs, while real test files only see NUnit's
// attributes. Compiling the test data (see Alignator.Tests.csproj) is what makes the
// seealso crefs between the formatting test classes and the test data resolve in the IDE.

using System;

namespace TBN.Alignator.Tests.test.data.CodeFormatter.CSharp.AlignArguments;

// Attribute named arguments are matched case-sensitively, so the properties carry the
// lowercase names the test data writes.
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
internal sealed class TestCaseAttribute : Attribute
{
    public TestCaseAttribute(params object[] args) { }

    public object name   { get; set; }
    public object diet   { get; set; }
    public object park   { get; set; }
    public object rex    { get; set; }
    public object raptor { get; set; }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
internal sealed class TestAttribute : Attribute
{
    public TestAttribute(params object[] args) { }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
internal sealed class TyrannosaurusTestCaseAttribute : Attribute
{
    public TyrannosaurusTestCaseAttribute(params object[] args) { }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
internal sealed class RaptorTestAttribute : Attribute
{
    public RaptorTestAttribute(params object[] args) { }
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
internal sealed class PachycephalosaurusTestCaseAttribute : Attribute
{
    public PachycephalosaurusTestCaseAttribute(params object[] args) { }
}
