namespace VariableParsingLibrary.Tools;

public class Tokenizer
{
    public List<string> Tokenize(string expression)
    {
        var tokens = new List<string>();
        var currentToken = string.Empty;

        for (int i = 0; i < expression.Length; i++)
        {
            var ch = expression[i];
            if (char.IsLetterOrDigit(ch) || ch == '.')
            {
                if (char.IsLetter(ch) && currentToken.Length > 0 && char.IsDigit(currentToken[currentToken.Length - 1]))
                {
                    tokens.Add(currentToken);
                    currentToken = string.Empty;
                }

                currentToken += ch;
            }
            else if (i < expression.Length - 1 && char.IsDigit(ch) && char.IsLetter(expression[i + 1]))
            {
                tokens.Add(currentToken);
                tokens.Add("*");

                currentToken = string.Empty;
            }
            else if (!string.IsNullOrEmpty(currentToken))
            {
                tokens.Add(currentToken);
                currentToken = string.Empty;
            }
            else if (!char.IsWhiteSpace(ch))
            {
                tokens.Add(ch.ToString());
            }
        }

        if (!string.IsNullOrEmpty(currentToken))
        {
            tokens.Add(currentToken);
        }

        return tokens;
    }
}