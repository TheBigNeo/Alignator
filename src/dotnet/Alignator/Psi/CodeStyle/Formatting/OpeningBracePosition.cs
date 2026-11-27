namespace TBN.Alignator.Psi.CodeStyle.Formatting;

/// <summary>
///     Position of the opening-brace alignment of object initializers
///     inside collections: the value of the <c>ALIGN_OBJECT_INITIALIZER_OPENING_BRACES</c> setting.
///     Padding before the brace lines up the <c>{</c> of every entry into one column,
///     padding after it the first members following them.
///     The independent <see cref="AlignPosition"/> comma settings of the entry's constructor arguments
///     stay untouched - both settings can be active at the same time.
/// </summary>
public enum OpeningBracePosition
{
    DoNotChange,
    SpaceBeforeBrace,
    SpaceAfterBrace,
}
