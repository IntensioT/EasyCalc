using FunctionComposeLibrary;
using System.Reflection;
using Xunit.Abstractions;

namespace EasyCalc.Tests
{
    public class FunctionComposer_SmokeTests
    {
        private FunctionComposer _composer;
        private ITestOutputHelper _output;
        public FunctionComposer_SmokeTests(ITestOutputHelper output)
        {
            _composer = new();
            _output = output;
        }

        [Fact]
        public void CheckSignatureDivisionToParameters()
        {
            var method = _composer.GetType().GetMethod("GetSignatureParameters", BindingFlags.NonPublic | BindingFlags.Instance);
            var expected = new string[] { "x", "y" };

            var actual = method?.Invoke(_composer, new[] { "f(x,y)" });

            Assert.NotNull(method);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void CheckSignatureWithSpacesDivisionToParameters()
        {
            var method = _composer.GetType().GetMethod("GetSignatureParameters", BindingFlags.NonPublic | BindingFlags.Instance);
            var expected = new string[] { "x", "y" };

            var actual = method?.Invoke(_composer, new[] { "f( x, y )" });

            Assert.NotNull(method);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void ParseFunctionBodyToPostfixForm()
        {
            var method = _composer.GetType().GetMethod("ParseToPostfixForm", BindingFlags.NonPublic | BindingFlags.Instance);
            var dividerField = _composer.GetType().GetField("_divider", BindingFlags.NonPublic | BindingFlags.Static);
            var divider = dividerField?.GetValue(_composer) ?? "";
            var expected = $"8{divider}2{divider}5{divider}*+1{divider}3{divider}2{divider}*+4{divider}-/";

            var actual = method?.Invoke(_composer, new[] { "(8+2*5)/(1+3*2-4)" }) as string;

            Assert.NotNull(dividerField);
            Assert.NotNull(method);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ParseFunctionBodyToPostfixFormXY()
        {
            var method = _composer.GetType().GetMethod("ParseToPostfixForm", BindingFlags.NonPublic | BindingFlags.Instance);
            var dividerField = _composer.GetType().GetField("_divider", BindingFlags.NonPublic | BindingFlags.Static);
            var divider = dividerField?.GetValue(_composer) ?? "";
            var expected = $"x,y,+";

            var actual = method?.Invoke(_composer, new[] { "x+y" }) as string;

            Assert.NotNull(dividerField);
            Assert.NotNull(method);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CheckGettingFunctionName()
        {
            var method = _composer.GetType().GetMethod("GetFunctionName", BindingFlags.NonPublic | BindingFlags.Instance);
            var expected = "func";

            var actual = method?.Invoke(_composer, new[] { "func( x, y )" });

            Assert.NotNull(method);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(2, 4)]
        [InlineData(2.4, 4.1)]
        [InlineData(5, 1)]
        public void SumOfXY(double x, double y)
        {
            var expected = x + y;
            var function = _composer.CreateFunction("f(x,y)", "x+y");

            var actual = function?.DynamicInvoke(new double[] { x, y });

            Assert.NotNull(function);
            Assert.Equal(expected, actual);
        }
        [Theory]
        [InlineData(2, 4, 1)]
        [InlineData(2.4, 4.1, 1)]
        [InlineData(5, 1, 1)]
        public void FunctionWith3Variables(double x, double y, double z)
        {
            var expected = x + y - z;
            var function = _composer.CreateFunction("f(x,y,z)", "x+y-z");

            var actual = function?.DynamicInvoke(new double[] { x, y, z });

            Assert.NotNull(function);
            Assert.Equal(expected, actual);
        }
        [Theory]
        [InlineData(2, 4, 1)]
        [InlineData(2.4, 4.1, 1)]
        [InlineData(5, 1, 1)]
        public void FunctionWith3VariablesAndConstant(double x, double y, double z)
        {
            const int value = 10;
            var expected = x + y - z + value;
            var function = _composer.CreateFunction("f(x,y,z)", $"x+y-z + {value}");

            var actual = function?.DynamicInvoke(new double[] { x, y, z });

            Assert.NotNull(function);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(12, 11, 1)]
        [InlineData(-12, 11, 1)]
        public void FunctionWithBracketsAndConstants(double x, double y, double z)
        {
            var expected = (x + 3) / (y + 5 * (3 - z));
            var function = _composer.CreateFunction("f(x,y,z)", $"(x + 3) / (y + 5 * (3 - 1))");

            var actual = function?.DynamicInvoke(new double[] { x, y, z });

            Assert.NotNull(function);
            Assert.Equal(expected, actual);
        }
        [Fact]
        public void GetCreatedFunction()
        {
            var method = _composer.CreateFunction("f(x,y)", $"x * x + y - 5");
            var savedMethod = _composer.GetFunction("f");
            int x = 2, y = 3;
            double expected = x * x + y - 5;

            var actual = savedMethod?.DynamicInvoke(new double[] { x, y });

            Assert.NotNull(method);
            Assert.NotNull(savedMethod);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateFunctionWithNoParameters()
        {
            var expected = 10;
            var function = _composer.CreateFunction("f()", $"(10 + 2 - (1 + 1))");

            var actual = _composer.CallFunction("f", new double[] { });

            Assert.NotNull(function);
            Assert.Equal(expected, actual);
        }
    }
}