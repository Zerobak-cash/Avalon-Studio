// Patch: make duplicated types partial to avoid CS0260 when building multiple partial sources.
// Place this file to: debugger-libs/Mono.Debugging/Mono.Debugging.Evaluation/Compat/EvaluationCompat.cs
using System;
namespace Mono.Debugging.Evaluation
{
    // previously: public class EvaluationContext
    public partial class EvaluationContext
    {
        // existing members preserved in other parts; this partial declaration ensures no duplicate-type errors.
    }

    // previously: public class EvaluationStatistics
    public partial class EvaluationStatistics
    {
        // partial placeholder
        public int Evaluations { get; set; }
    }

    // previously: public class ExpressionEvaluator
    public partial class ExpressionEvaluator
    {
        public object Evaluate(string expr) => null!;
    }

    // previously: public class EvaluatorExceptionThrownException
    public partial class EvaluatorExceptionThrownException : Exception
    {
        public EvaluatorExceptionThrownException(string message) : base(message) { }
    }

    // previously: public class EvaluationObjectValue
    public partial class EvaluationObjectValue
    {
        public object? Value { get; set; }
        public override string ToString() => Value?.ToString() ?? string.Empty;
    }
}
