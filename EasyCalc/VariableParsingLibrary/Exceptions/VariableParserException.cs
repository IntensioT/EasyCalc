namespace VariableParsingLibrary.Exceptions;

public class VariableParserException : Exception
{
    public VariableParserException() { }
    
    public VariableParserException(string message) 
        : base(message) { }
    
    public VariableParserException(string message, Exception e) 
        : base(message, e) { }
}