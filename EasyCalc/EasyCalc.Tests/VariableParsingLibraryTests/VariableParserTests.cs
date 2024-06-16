using VariableParsingLibrary;
using VariableParsingLibrary.Exceptions;

namespace EasyCalc.Tests.VariableParsingLibraryTests;

public class VariableParserTests
{
    [Theory]
    [InlineData("x = 2", "y = x + 2", 4)]       // x = 2, y = 4
    [InlineData("x = 2.4", "y = x + 1.7", 4.1)] // x = 2.4, y = 4.1
    [InlineData("x = 5", "y = x - 4", 1)]       // x = 5, y = 1
    public void GetVariableValue_ReturnsCorrectValue(string xDeclaration, string yDeclaration, double expectedY)
    {
        // Arrange
        var variableDeclarations = new List<string> { xDeclaration, yDeclaration };
        var parser = new VariableParser(variableDeclarations);

        // Act
        var yValue = parser.GetVariableValue("y");

        // Assert
        Assert.Equal(expectedY, yValue);
    }

    [Fact]
    public void UndefinedVariable_ThrowsException()
    {
        // Arrange
        var variableDeclarations = new List<string>()
        {
            "y = x + 1"
        };

        // Act & Assert
        Assert.Throws<VariableParserException>(() => new VariableParser(variableDeclarations));
    }

    [Fact]
    public void CircularDependency_ThrowsException()
    {
        // Arrange
        var variableDeclarations = new List<string>()
        {
            "x = y + 1",
            "y = x + 1"
        };
        var parser = new VariableParser(variableDeclarations);

        // Act & Assert
        Assert.Throws<VariableParserException>(() => parser.GetVariableValue("x"));
    }
    
    [Theory]
    [InlineData("1x = 2")]
    [InlineData("var! = 2")]
    [InlineData("x-y = 2")]
    [InlineData("x y = 2")]
    public void InvalidVariableName_ThrowsException(string invalidDeclaration)
    {
        // Arrange
        var variableDeclarations = new List<string> { invalidDeclaration };

        // Act & Assert
        Assert.Throws<VariableParserException>(() => new VariableParser(variableDeclarations));
    }
}