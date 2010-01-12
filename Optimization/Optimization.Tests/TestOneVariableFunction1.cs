
namespace Optimization.Tests
{
    using NUnit.Framework;
    using Optimization.Methods.ZerothOrder.OneVariable;

    [TestFixture]
    class TestOneVariableFunction1
    {
        double eps;
        OneVariableFunction function;
        double a0;
        double b0;
        double result;

        [SetUp]
        public void UpTestMethod()
        {
            eps = 0.01;
            function = delegate(double x)
            {
                return (2 * x * x - 12 * x);
            };
            a0 = 0;
            b0 = 10;
            result = 3;
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
