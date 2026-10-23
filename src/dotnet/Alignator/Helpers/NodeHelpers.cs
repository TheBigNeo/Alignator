using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace TBN.Alignator.Helpers;

public static class NodeHelpers
{
    public static long? GetContainingBlockOffset(this ITreeNode node)
    {
        var n = node;
        while (n != null)
        {
            if (n is IBlock block)
                return block.GetDocumentStartOffset().Offset;
            n = n.Parent;
        }

        n = node;
        while (n != null)
        {
            if (n is ICSharpTypeDeclaration typeDeclaration)
                return typeDeclaration.GetDocumentStartOffset().Offset;
            n = n.Parent;
        }

        return null;
    }
}