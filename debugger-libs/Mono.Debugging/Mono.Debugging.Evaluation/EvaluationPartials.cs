using System;

namespace Mono.Debugging.Evaluation
{
    // Public partial types to link with existing partial declarations in other files
    public partial class EvaluationContext
    {
        // placeholder members (no behavior)
    }

    public partial class EvaluationStatistics
    {
        public int Evaluations { get; set; }
    }

    public abstract partial class ExpressionEvaluator
    {
        public abstract object Evaluate(string expression, EvaluationContext context);
    }

    public partial class EvaluatorExceptionThrownException : Exception
    {
        public EvaluatorExceptionThrownException() { }
        public EvaluatorExceptionThrownException(string message) : base(message) { }
    }

    public partial class ObjectValue
    {
        public object Value { get; set; }
        public ObjectValue() { }
        public ObjectValue(object v) => Value = v;
        public override string ToString() => Value?.ToString() ?? "null";
    }
}
