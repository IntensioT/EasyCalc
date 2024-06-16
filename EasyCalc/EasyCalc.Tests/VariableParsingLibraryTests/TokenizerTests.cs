using VariableParsingLibrary.Tools;

namespace EasyCalc.Tests.VariableParsingLibraryTests;

public class TokenizerTests
{
    [Fact]
    public void Tokenize_HandlesBasicExpression()
    {
        // Arrange
        var tokenizer = new Tokenizer();
        var expression = "2 + 3";

        // Act
        var tokens = tokenizer.Tokenize(expression);

        // Assert
        var expectedTokens = new List<string> { "2", "+", "3" };
        Assert.Equal(expectedTokens, tokens);
    }

    [Fact]
    public void Tokenize_HandlesExponentialNotation()
    {
        // Arrange
        var tokenizer = new Tokenizer();
        var expression = "2.5e2 + 3";

        // Act
        var tokens = tokenizer.Tokenize(expression);

        // Assert
        var expectedTokens = new List<string> { "2.5e2", "+", "3" };
        Assert.Equal(expectedTokens, tokens);
    }

    [Fact]
    public void Tokenize_HandlesImplicitMultiplication()
    {
        // Arrange
        var tokenizer = new Tokenizer();
        var expression = "2x";

        // Act
        var tokens = tokenizer.Tokenize(expression);

        // Assert
        var expectedTokens = new List<string> { "2", "*", "x" };
        Assert.Equal(expectedTokens, tokens);
    }
}