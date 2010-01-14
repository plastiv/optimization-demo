namespace Optimization.Tests
{
    using NUnit.Framework;
    using Optimization.Methods;
    using Optimization.Tests.Tasks;

    [TestFixture(0)]
    [TestFixture(1)]
    [TestFixture(2)]
    [TestFixture(3)]
    [TestFixture(4)]
    [TestFixture(5)]
    [TestFixture(6)]
    [TestFixture(7)]
    [TestFixture(8)]
    [TestFixture(9)]
    [TestFixture(10)]
    //[Ignore]
    class TestManyVariableMethods
    {
        private const double precision = 0.1;
        private readonly ManyVariableFunctionTask task;

        public TestManyVariableMethods(int taskNum)
        {
            switch (taskNum)
            {
                case 0:
                    task = new ManyVariableFunctionTask0();
                    break;

                case 1:
                    task = new ManyVariableFunctionTask1();
                    break;

                case 2:
                    task = new ManyVariableFunctionTask2();
                    break;

                case 3:
                    task = new ManyVariableFunctionTask3();
                    break;

                case 4:
                    task = new ManyVariableFunctionTask4();
                    break;

                case 5:
                    task = new ManyVariableFunctionTask5();
                    break;

                case 6:
                    task = new ManyVariableFunctionTask6();
                    break;

                case 7:
                    task = new ManyVariableFunctionTask7();
                    break;

                case 8:
                    task = new ManyVariableFunctionTask8();
                    break;

                case 9:
                    task = new ManyVariableFunctionTask9();
                    break;

                case 10:
                    task = new ManyVariableFunctionTask10();
                    break;

                default:
                    break;
            }
        }

        [Test]        
        public void TestGradientMethod()
        {
            double[] result = Minimum.GradientDescent(this.task.function, 2, this.task.startPoint);
            Assert.AreEqual(this.task.exactSolution[0], result[0], precision);
            Assert.AreEqual(this.task.exactSolution[1], result[1], precision);
        }

        [Test]
        [Ignore]
        public void TestDeformablePolyhedronMethod()
        {
            double[] result = Minimum.DeformablePolyhedron(this.task.function, 2, this.task.startPoint);
            Assert.AreEqual(this.task.exactSolution[0], result[0], precision);
            Assert.AreEqual(this.task.exactSolution[1], result[1], precision);
        }

        [Test]
        [Ignore]
        public void TestHookeJeveesMethod()
        {
            double[] result = Minimum.HookeJevees(this.task.function, 2, this.task.startPoint);
            Assert.AreEqual(this.task.exactSolution[0], result[0], precision);
            Assert.AreEqual(this.task.exactSolution[1], result[1], precision);
        }

        [Test]
        [Ignore]
        public void TestRandomMethod()
        {
            double[] result = Minimum.Random(this.task.function, 2, this.task.startPoint);
            Assert.AreEqual(this.task.exactSolution[0], result[0], precision);
            Assert.AreEqual(this.task.exactSolution[1], result[1], precision);
        }

        [Test]
        [Ignore]
        public void TestRosenbrockMethod()
        {
            double[] result = Minimum.Rosenbrock(this.task.function, 2, this.task.startPoint);
            Assert.AreEqual(this.task.exactSolution[0], result[0], precision);
            Assert.AreEqual(this.task.exactSolution[1], result[1], precision);
        }
    }
}