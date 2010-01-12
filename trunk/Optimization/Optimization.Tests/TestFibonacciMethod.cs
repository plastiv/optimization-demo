
namespace Optimization.Tests
{
    using NUnit.Framework;
    using Optimization.Methods.ZerothOrder.OneVariable;
    using System;

    [TestFixture]
    class TestFibonacciMethod
    {
        const double _a = 5;
        const double _b = 2.3;
        const double _c = 0.1;
        const double _d = 0.7;

        [Test]
        public void TestMethodWork1()
        {
            OneVariableFunction ovf = delegate(double x)
            {
                return (2 * x * x - 12 * x);
            };
            double a0 = 0;
            double b0 = 10;

            Assert.AreEqual(3,Fibonacci.GetMinimum(ovf, a0, b0, 0.01),0.01);
        }

        [Test]
        public void TestMethodWork2()
        {
            OneVariableFunction ovf = delegate(double x)
            {
                return (2 * _a * x * x + 16 * _b / x);
            };
            double a0 = 1;
            double b0 = 5;

            Assert.AreEqual(1.23, Fibonacci.GetMinimum(ovf, a0, b0, 0.01), 0.01);
        }

        [Test]
        public void TestMethodWork3()
        {
            OneVariableFunction ovf = delegate(double x)
            {
                return (5 * _a * Math.Pow(x, 6) - 36 * _b * Math.Pow(x, 5) + 82.5 * _c * Math.Pow(x, 4) -
                    60 * _d * Math.Pow(x, 3) + 36);
            };
            double a0 = 0.2;
            double b0 = 1.6;

            Assert.AreEqual(1.6, Fibonacci.GetMinimum(ovf, a0, b0, 0.01), 0.01);
        }

        [Test]
        public void TestMethodWork4()
        {
            OneVariableFunction ovf = delegate(double x)
            {
                return (-_a * Math.Pow(x, 3) + 3 * _b * x * x + 9 * _c * x * 10);
            };
            double a0 = -1;
            double b0 = 4;

            Assert.AreEqual(4, Fibonacci.GetMinimum(ovf, a0, b0, 0.01), 0.01);
        }

    }
}
