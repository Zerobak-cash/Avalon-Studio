using System;

namespace Mono.Debugging.Evaluation
{
    public class EvaluationStatistics { public int Evaluations { get; set; } }
    public class EvaluationContext { }
    public abstract class ExpressionEvaluator { public abstract object Evaluate(string expression, EvaluationContext context); }
    public class EvaluatorExceptionThrownException : Exception { public EvaluatorExceptionThrownException() {} public EvaluatorExceptionThrownException(string m): base(m){} }
    // Make ObjectValue partial to avoid partial declaration conflicts
    public partial class ObjectValue { public object Value { get; set; } public ObjectValue(){} public ObjectValue(object v) => Value = v; public override string ToString() => Value?.ToString() ?? "null"; }
    public partial class ObjectValue // extra partial part to reduce conflicts if another part exists
    {
        public string TypeName => Value?.GetType().Name ?? "null";
    }
}
