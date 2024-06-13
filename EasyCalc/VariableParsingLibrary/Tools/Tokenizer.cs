namespace VariableParsingLibrary.Tools;

public class Tokenizer
{
    public List<string> Tokenize(string expression)
    {
        var tokens = new List<string>();
        var currentToken = string.Empty;
        bool isInExponent = false;

        for (int i = 0; i < expression.Length; i++)
        {
            var ch = expression[i];

            if (char.IsLetterOrDigit(ch) || ch == '.' || (isInExponent && (ch == '+' || ch == '-')))
            {
                if (char.IsLetter(ch))
                {
                    if (ch == 'e' || ch == 'E')
                    {
                        isInExponent = true;
                    }
                    else if (currentToken.Length > 0 && char.IsDigit(currentToken[currentToken.Length - 1]))
                    {
                        tokens.Add(currentToken);
                        currentToken = string.Empty;
                    }
                }
                currentToken += ch;
            }
            else
            {
                if (!string.IsNullOrEmpty(currentToken))
                {
                    tokens.Add(currentToken);
                    currentToken = string.Empty;
                }

                if (!char.IsWhiteSpace(ch))
                {
                    tokens.Add(ch.ToString());
                }
                isInExponent = false;
            }

            if (i < expression.Length - 1 && char.IsDigit(ch) && char.IsLetter(expression[i + 1]) && expression[i + 1] != 'e' && expression[i + 1] != 'E')
            {
                tokens.Add(currentToken);
                tokens.Add("*");
                currentToken = string.Empty;
                isInExponent = false;
            }
        }

        if (!string.IsNullOrEmpty(currentToken))
        {
            tokens.Add(currentToken);
        }

        return tokens;
    }
}
