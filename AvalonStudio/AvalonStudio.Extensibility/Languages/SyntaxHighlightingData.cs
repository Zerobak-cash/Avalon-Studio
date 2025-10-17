using AvalonStudio.Projects;
using System.Collections.Generic;

namespace AvalonStudio.Languages
{
    public enum HighlightType
    {
        None,
        CallExpression,
        Punctuation,
        ControlKeyword,
        Keyword,
        Identifier,
        Literal,
        NumericLiteral,
        Comment,
        ClassName,
        StructName,
        EnumConstant,
        EnumTypeName,
        InterfaceName,
        DelegateName,
        PreProcessor,
        PreProcessorText,
        Operator,
        ExcludedCode,
        Unnecessary
    }

    public class SyntaxHighlightDataList : List<OffsetSyntaxHighlightingData>
    {
        public new void Add(OffsetSyntaxHighlightingData item)
        {
            var index = BinarySearch(item);

            if (index < 0)
            {
                Insert(~index, item);
            }
            else
            {
                Insert(index + 1, item);
            }
        }
    }
}