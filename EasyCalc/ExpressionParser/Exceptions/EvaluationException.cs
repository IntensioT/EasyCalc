namespace ExpressionParser.Exceptions;

public class EvaluationException:Exception
{
    public EvaluationException(string message, Exception exception) : base(message)
    {
        
    }
    public EvaluationException(string message) : base(message)
    {
        
    }
}