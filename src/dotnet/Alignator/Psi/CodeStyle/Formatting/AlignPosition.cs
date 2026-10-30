namespace TBN.Alignator.Psi.CodeStyle.Formatting;

/// <summary>
///     Position of the argument alignment around the commas:
///     the values of the <c>ALIGN_*_ARGUMENTS</c> settings.
///     Padding before the comma lines up the commas,
///     padding after it the arguments following them.
///     The first argument is aligned by the independent <see cref="FirstArgumentPosition"/> parenthesis settings -
///     both settings can be active at the same time.
/// </summary>
public enum AlignPosition
{
    DoNotChange,
    SpaceBeforeComma,
    SpaceAfterComma,
}
