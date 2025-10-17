using System;
namespace Mono.Debugging.Evaluation
{
    public class EvaluationContext { }
    public class EvaluationStatistics { public int Evaluations { get; set; } }
    public abstract class ExpressionEvaluator { public abstract object Evaluate(string expression, EvaluationContext context); }
    public class EvaluatorExceptionThrownException : Exception { public EvaluatorExceptionThrownException() {} public EvaluatorExceptionThrownException(string msg): base(msg){} }
    // Avoid name collision with Client.ObjectValue by using EvaluationObjectValue
    public class EvaluationObjectValue { public object Value { get; set; } public override string ToString() => Value?.ToString() ?? "null"; }
}
