using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace TBN.Alignator.Helpers;

/// <summary>
///     PSI tree helpers shared by the Alignator formatting rules.
/// </summary>
public static class NodeHelpers
{
    /// <summary>
    ///     Returns the document start offset of the innermost enclosing <see cref="IBlock"/>,
    ///     or - when no block ancestor exists (e.g. for field/property initializers) -
    ///     of the innermost enclosing <see cref="ICSharpTypeDeclaration"/>.
    ///     Alignment rules use this offset to scope an alignment group to one block or type declaration.
    /// </summary>
    /// <param name="node">Node to start the ancestor search from.</param>
    /// <returns>The block (or type declaration) start offset, or <c>null</c> if neither is found.</returns>
    public static long? GetContainingBlockOffset(this ITreeNode node)
    {
        var n = node;
        while (n != null)
        {
            if (n is IBlock block)
            {
                return block.GetDocumentStartOffset().Offset;
            }

            n = n.Parent;
        }

        n = node;
        while (n != null)
        {
            if (n is ICSharpTypeDeclaration typeDeclaration)
            {
                return typeDeclaration.GetDocumentStartOffset().Offset;
            }

            n = n.Parent;
        }

        return null;
    }
}
