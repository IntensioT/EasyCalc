using VariableParsingLibrary.Tools;

namespace EasyCalc.Tests.VariableParsingLibraryTests;

public class InfixToPostfixConverterTests
{
    [Fact]
    public void Convert_HandlesBasicExpression()
    {
        // Arrange
        var converter = new InfixToPostfixConverter();
        var tokens = new List<string> { "2", "+", "3" };

        // Act
        var postfix = converter.Convert(tokens);

        // Assert
        var expectedPostfix = new List<string> { "2", "3", "+" };
        Assert.Equal(expectedPostfix, postfix);
    }

    [Fact]
    public void Convert_HandlesParentheses()
    {
        // Arrange
        var converter = new InfixToPostfixConverter();
        var tokens = new List<string> { "(", "2", "+", "3", ")", "*", "4" };

        // Act
        var postfix = converter.Convert(tokens);

        // Assert
        var expectedPostfix = new List<string> { "2", "3", "+", "4", "*" };
        Assert.Equal(expectedPostfix, postfix);
    }
}