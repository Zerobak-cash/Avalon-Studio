using System;

namespace Mono.Debugging.Evaluation
{
    // Final partials to ensure linking with other partial declarations
    public partial class EvaluationContext
    {
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
