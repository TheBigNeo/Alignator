namespace TBN.Alignator.Psi.CodeStyle.Formatting;

/// <summary>
///     Position of the first-argument alignment of constructor, function and attribute calls.
///     Independent of the <see cref="AlignPosition"/> comma settings: both settings can be
///     active at the same time - the first-argument setting pads the parenthesis gap, the
///     comma setting pads the comma gaps.
/// </summary>
public enum FirstArgumentPosition
{
    DoNotChange,
    SpaceBeforeParenthese,
    SpaceAfterParenthese,
}
