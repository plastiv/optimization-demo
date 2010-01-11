
namespace Optimization.Tests
{
    using NUnit.Framework;
    using Optimization.Methods.ZerothOrder.OneVariable;

    [TestFixture]
    class TestFibonacciMethod
    {
        [Test]
        public void TestMethodWork1()
        {
            OneVariableFunction ovf = delegate(double x)
            {
                return (2 * x * x - 12 * x);
            };
            double a0 = 0;
            double b0 = 10;

            Assert.AreEqual(2.697,Fibonacci.GetMinimum(ovf, a0, b0, 0.01),0.01);
        }

    }
}
