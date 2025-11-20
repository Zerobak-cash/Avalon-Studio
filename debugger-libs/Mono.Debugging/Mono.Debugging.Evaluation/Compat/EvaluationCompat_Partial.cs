// Companion partial types that complement other partial declarations.
// This file intentionally contains partial type members.
using System;
namespace Mono.Debugging.Evaluation
{
    public partial class EvaluationContext
    {
        public string Name { get; set; } = string.Empty;
    }

    public partial class EvaluationStatistics
    {
        // keep single definition of Evaluations here as well; partial keyword avoids duplicate-type error.
        public int Evaluations { get; set; }
    }

    public partial class ExpressionEvaluator
    {
        public object Evaluate(string expr, EvaluationContext context) => null!;
    }

    public partial class EvaluationObjectValue
    {
        // Value already declared in another partial; duplication avoided thanks to 'partial' modifier.
    }
}
