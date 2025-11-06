using System;
namespace Mono.Debugging.Evaluation
{
    public partial class EvaluationContext { }
    public partial class EvaluationStatistics { }
    public partial class EvaluationObjectValue { public object Value { get; set; } }
    public class EvaluatorExceptionThrownException : Exception { public EvaluatorExceptionThrownException(){} public EvaluatorExceptionThrownException(string m): base(m){} }
    public abstract partial class ExpressionEvaluator { }
}
