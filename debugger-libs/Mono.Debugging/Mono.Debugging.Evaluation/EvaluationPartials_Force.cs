using System;

namespace Mono.Debugging.Evaluation
{
    // Force-included partials to ensure project compiles.
    public partial class EvaluationContext { }
    public partial class EvaluationStatistics { public int Evaluations { get; set; } }
    public abstract partial class ExpressionEvaluator { public abstract object Evaluate(string expression, EvaluationContext context); }
    public partial class EvaluatorExceptionThrownException : Exception { public EvaluatorExceptionThrownException() {} public EvaluatorExceptionThrownException(string m): base(m){} }
    public partial class EvaluationObjectValue { public object Value { get; set; } public EvaluationObjectValue(){} public EvaluationObjectValue(object v) => Value = v; public override string ToString() => Value?.ToString() ?? "null"; }
}
