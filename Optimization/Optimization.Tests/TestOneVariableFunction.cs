
namespace Optimization.Tests
{
    using System;
    using NUnit.Framework;
    using Optimization.Methods.ZerothOrder.OneVariable;

    [TestFixture(0, 0, 10, 0.01, 3)]
    [TestFixture(1, 1, 5, 0.01, 1.23)]
    [TestFixture(2, 0.2, 1.6, 0.01, 1.6)]
    [TestFixture(3, -1, 4, 0.01, 4)]
    [Ignore]
    class TestOneVariableFunction
    {
        private readonly double eps;
        private readonly OneVariableFunction function;
        private readonly double a0;
        private readonly double b0;
        private readonly double result;

        private const double _a = 5;
        private const double _b = 2.3;
        private const double _c = 0.1;
        private const double _d = 0.7;

        public TestOneVariableFunction(int function, double leftBound, double rightBound, double precision, double result)
        {
            switch (function)
            {
                case 0:
                    this.function = delegate(double x)
                    {
                        return (2 * x * x - 12 * x);
                    };
                    break;
                case 1:
                    this.function = delegate(double x)
                    {
                        return (2 * _a * x * x + 16 * _b / x);
                    };
                    break;
                case 2:
                    this.function = delegate(double x)
                    {
                        return (5 * _a * Math.Pow(x, 6) - 36 * _b * Math.Pow(x, 5) + 82.5 * _c * Math.Pow(x, 4) -
                            60 * _d * Math.Pow(x, 3) + 36);
                    };
                    break;
                case 3:
                    this.function = delegate(double x)
                    {
                        return (-_a * Math.Pow(x, 3) + 3 * _b * x * x + 9 * _c * x * 10);
                    };
                    break;
                default:
                    break;
            }
            
            this.eps = precision;
            a0 = leftBound;
            b0 = rightBound;
            this.result = result;
        }




        [Test]
        public void TestBisection()
        {
            Assert.AreEqual(result, Bisection.GetMinimum(function, a0, b0, eps), eps);
        }

        [Test]
        public void TestDichotomy()
        {
            Assert.AreEqual(result, Dichotomy.GetMinimum(function, a0, b0, eps), eps);
        }

        [Test]
        public void TestFibonacci()
        {
            Assert.AreEqual(result, Fibonacci.GetMinimum(function, a0, b0, eps), eps);
        }

        [Test]
        public void TestGoldenSection()
        {
            Assert.AreEqual(result, GoldenSection.GetMinimum(function, a0, b0, eps), eps);
        }

        [Test]
        public void TestModifedGoldenSection()
        {
            Assert.AreEqual(result, ModifedGoldenSection.GetMinimum(function, a0, b0, eps), eps);
        }

        [Test]
        public void TestModifedUniform()
        {
            Assert.AreEqual(result, ModifedUniform.GetMinimum(function, a0, b0, eps), eps);
        }

        [Test]
        public void TestQuadraticInterpolation()
        {
            Assert.AreEqual(result, QuadraticInterpolation.GetMinimum(function, a0, b0, eps), eps);
        }

        [Test]
        public void TestTernarySearch()
        {
            Assert.AreEqual(result, TernarySearch.GetMinimum(function, a0, b0, eps), eps);
        }

        [Test]
        public void TestUniform()
        {
            Assert.AreEqual(result, Uniform.GetMinimum(function, a0, b0, eps), eps);
        }

    }
}
