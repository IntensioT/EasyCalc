using System.Reflection;
using ExpressionParser;
using ExpressionParser.Exceptions;
using VariableParsingLibrary.Exceptions;

public class ExpressionHandler_Tests
{
    [Fact]
    public void EvaluateExpression_SingleFunction_CorrectResult()
    {
        // Arrange
        var variableDeclarations = new List<string> { "x=2", "y=x+9" };
        var functionsList = new List<string> { "f(x,y)=x+y" };
        var expressionHandler = new ExpressionHandler(variableDeclarations, functionsList);

        var expression = "f(2,4) - 7";

        // Act
        var result = expressionHandler.EvaluateExpression(expression);

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public void EvaluateExpression_MultipleFunctionsAndVariables_CorrectResult()
    {
        // Arrange
        var variableDeclarations = new List<string> { "x=2", "y=x+9" };
        var functionsList = new List<string> { "f(x,y)=x+y", "g(z)=z*2" }; // Correct functions list
        var expressionHandler = new ExpressionHandler(variableDeclarations, functionsList);

        var expression = "f(x,y) + g(x)";

        // Act
        var result = expressionHandler.EvaluateExpression(expression);

        // Assert
        Assert.Equal(17.0, result);
    }

    [Fact]
    public void EvaluateExpression_InvalidFunctionDefinition_ExceptionThrown()
    {
        // Arrange
        var variableDeclarations = new List<string> { "x=2", "y=x+9" };
        var functionsList = new List<string> { "f(x,y,z)=x+y+z" }; // Incorrect function definition
        var expressionHandler = new ExpressionHandler(variableDeclarations, functionsList);

        var expression = "f(x,y) - 7";

        // Act & Assert
        Assert.Throws<TargetInvocationException>(() => expressionHandler.EvaluateExpression(expression));
    }


    [Fact]
    public void EvaluateExpression_CircularDependencyInVariables_ExceptionThrown()
    {
        // Arrange
        var variableDeclarations = new List<string> { "x=y", "y=x+9" }; // Circular dependency
        var functionsList = new List<string>();
        var expressionHandler = new ExpressionHandler(variableDeclarations, functionsList);

        var expression = "x - y";

        // Act & Assert
        Assert.Throws<VariableParserException>(() => expressionHandler.EvaluateExpression(expression));
    }
      [Fact]
    public void EvaluateExpression_UndefinedVariable_ExceptionThrown()
    {
        // Arrange
        var variableDeclarations = new List<string> { "x=2", "y=x+9" };
        var functionsList = new List<string> { "f(x,y)=x+y" };
        var expressionHandler = new ExpressionHandler(variableDeclarations, functionsList);

        var expression = "f(x,z)";

        // Act & Assert
        Assert.Throws<EvaluationException>(() => expressionHandler.EvaluateExpression(expression));
    }
    
    [Fact]
    public void EvaluateExpression_InvalidExpression_ExceptionThrown()
    {
        // Arrange
        var variableDeclarations = new List<string> { "x=2", "y=x+9" };
        var functionsList = new List<string> { "f(x,y)=x+y" };
        var expressionHandler = new ExpressionHandler(variableDeclarations, functionsList);

        var expression = "f(x, y";

        // Act & Assert
        Assert.Throws<OperationException>(() => expressionHandler.EvaluateExpression(expression));
    }

    [Fact]
    public void EvaluateExpression_DivisionByZero_ExceptionThrown()
    {
        // Arrange
        var variableDeclarations = new List<string> { "x=2", "y=0" };
        var functionsList = new List<string>();
        var expressionHandler = new ExpressionHandler(variableDeclarations, functionsList);

        var expression = "x / y";

        // Act & Assert
        Assert.Throws<EvaluationException>(() => expressionHandler.EvaluateExpression(expression));
    }



    [Fact]
    public void EvaluateExpression_ComplexExpression_CorrectResult()
    {
        // Arrange
        var variableDeclarations = new List<string> { "x=2", "y=3", "z=x+y" };
        var functionsList = new List<string> { "f(x,y,z)=x*y+z" }; // Complex function
        var expressionHandler = new ExpressionHandler(variableDeclarations, functionsList);

        var expression = "f(x,y,z)";

        // Act
        var result = expressionHandler.EvaluateExpression(expression);
        // Assert
        Assert.Equal(11.0, result);
    }

    [Fact]
    public void EvaluateExpression_EmptyVariableList_CorrectResult()
    {
        // Arrange
        var variableDeclarations = new List<string>();
        var functionsList = new List<string> { "f(x)=x" };
        var expressionHandler = new ExpressionHandler(variableDeclarations, functionsList);

        var expression = "f(5)";

        // Act
        var result = expressionHandler.EvaluateExpression(expression);

        // Assert
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void EvaluateExpression_EmptyFunctionList_CorrectResult()
    {
        // Arrange
        var variableDeclarations = new List<string> { "x=2" };
        var functionsList = new List<string>();
        var expressionHandler = new ExpressionHandler(variableDeclarations, functionsList);

        var expression = "x + 3";

        // Act
        var result = expressionHandler.EvaluateExpression(expression);

        // Assert
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void EvaluateExpression_ComplexExpressionWithParentheses_CorrectResult()
    {
        // Arrange
        var variableDeclarations = new List<string> { "x=2", "y=3" };
        var functionsList = new List<string>();
        var expressionHandler = new ExpressionHandler(variableDeclarations, functionsList);

        var expression = "x * (y + 1)";

        // Act
        var result = expressionHandler.EvaluateExpression(expression);

        // Assert
        Assert.Equal(8.0, result);
    }
     [Fact]
    public void EvaluateExpression_SingleVariable_CorrectResult()
    {
        // Arrange
        var variableDeclarations = new List<string> { "x=2" };
        var functionsList = new List<string>();
        var expressionHandler = new ExpressionHandler(variableDeclarations, functionsList);

        var expression = "x";

        // Act
        var result = expressionHandler.EvaluateExpression(expression);

        // Assert
        Assert.Equal(2.0, result);
    }

    [Fact]
    public void EvaluateExpression_SingleFunctionWithVariables_CorrectResult()
    {
        // Arrange
        var variableDeclarations = new List<string> { "x=2", "y=3" };
        var functionsList = new List<string> { "f(x,y)=x*y" };
        var expressionHandler = new ExpressionHandler(variableDeclarations, functionsList);

        var expression = "f(x,y)";

        // Act
        var result = expressionHandler.EvaluateExpression(expression);

        // Assert
        Assert.Equal(6.0, result);
    }

    [Fact]
    public void EvaluateExpression_FunctionWithNestedFunctions_CorrectResult()
    {
        // Arrange
        var variableDeclarations = new List<string> { "x=2", "y=3", "z=x+y" };
        var functionsList = new List<string> { "f(x,y,z)=x*y+z", "g(a,b)=a-b" };
        var expressionHandler = new ExpressionHandler(variableDeclarations, functionsList);

        var expression = "g(f(x,y,z), z)";

        // Act
        var result = expressionHandler.EvaluateExpression(expression);

        // Assert
        Assert.Equal(6.0, result);
    }

    [Fact]
    public void EvaluateExpression_VariableReassignment_CorrectResult()
    {
        // Arrange
        var variableDeclarations = new List<string> { "x=2", "y=x+3", "x=5" }; // Reassigning x
        var functionsList = new List<string>();
        var expressionHandler = new ExpressionHandler(variableDeclarations, functionsList);

        var expression = "y";

        // Act
        var result = expressionHandler.EvaluateExpression(expression);

        // Assert
        Assert.Equal(8.0, result);
    }

    [Fact]
    public void EvaluateExpression_EmptyFunctionBody_CorrectResult()
    {
        // Arrange
        var variableDeclarations = new List<string> { "x=2" };
        var functionsList = new List<string> { "f(x,)=x*2" }; // Empty function body
        var expressionHandler = new ExpressionHandler(variableDeclarations, functionsList);

        var expression = "f(x)";

        var result = expressionHandler.EvaluateExpression(expression);
        // Act & Assert
        Assert.Equal(4.0, result);
    }

    [Fact]
    public void EvaluateExpression_UndefinedFunction_ExceptionThrown()
    {
        // Arrange
        var variableDeclarations = new List<string> { "x=2" };
        var functionsList = new List<string> { "f(x)=x*2" };
        var expressionHandler = new ExpressionHandler(variableDeclarations, functionsList);

        var expression = "g(x)";

        // Act & Assert
        Assert.Throws<EvaluationException>(() => expressionHandler.EvaluateExpression(expression));
    }

    [Fact]
    public void EvaluateExpression_EmptyVariableDeclaration_CorrectResult()
    {
        // Arrange
        var variableDeclarations = new List<string>();
        var functionsList = new List<string> { "f(x)=x*2" };
        var expressionHandler = new ExpressionHandler(variableDeclarations, functionsList);

        var expression = "f(5)";

        // Act
        var result = expressionHandler.EvaluateExpression(expression);

        // Assert
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void EvaluateExpression_ComplexExpressionWithFunctions_CorrectResult()
    {
        // Arrange
        var variableDeclarations = new List<string> { "x=2", "y=3" };
        var functionsList = new List<string> { "f(x,y)=x*y", "g(a,b)=a+b" };
        var expressionHandler = new ExpressionHandler(variableDeclarations, functionsList);

        var expression = "f(x,y) + g(x,y)";

        // Act
        var result = expressionHandler.EvaluateExpression(expression);

        // Assert
        Assert.Equal(11.0, result);
    }

    [Fact]
    public void EvaluateExpression_ExpressionWithNoVariables_CorrectResult()
    {
        // Arrange
        var variableDeclarations = new List<string>();
        var functionsList = new List<string>();
        var expressionHandler = new ExpressionHandler(variableDeclarations, functionsList);

        var expression = "2 * 3 + 4";

        // Act
        var result = expressionHandler.EvaluateExpression(expression);

        // Assert
        Assert.Equal(10.0, result);
    }
    
}
