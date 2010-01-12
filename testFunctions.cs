namespace Optimization.VisualApplication
{
    using OptimizationMethods;
    internal class TestFunctions
    {
        public int funcCount;
        public ManyVariable[] funcs;
        public double[][] startingPoint;
        public double[][] exactSolution;
        public int[] funcDimension;
        public string[] funcsStr;

        public TestFunctions()
        {
            funcCount = 11;
            funcs = new ManyVariable[funcCount];
            startingPoint = new double[funcCount][];
            exactSolution = new double[funcCount][];
            funcDimension = new int[funcCount];
            funcsStr = new string[funcCount];
            int funcNum;

            ManyVariable function0 = delegate(double[] x)
            {
                return x[0] * x[0] * x[0] - x[0] * x[1] + x[1] * x[1] - 2 * x[0] + 3 * x[1] - 4;
            };
            funcNum = 0;
            funcDimension[funcNum] = 2;
            startingPoint[funcNum] = new double[funcDimension[funcNum]];
            startingPoint[funcNum][0] = 0;
            startingPoint[funcNum][1] = 0;
            exactSolution[funcNum] = new double[funcDimension[funcNum]];
            exactSolution[funcNum][0] = 0.5;
            exactSolution[funcNum][1] = -1.25;
            funcs[funcNum] = function0;
            funcsStr[funcNum] = "x[0] * x[0] * x[0] - x[0] * x[1] + x[1] * x[1] - 2 * x[0] + 3 * x[1] - 4";

            ManyVariable function1 = delegate(double[] x)
            {
                return (x[1] - x[0] * x[0]) * (x[1] - x[0] * x[0]) + (1 - x[0]) * (1 - x[0]);
            };
            funcNum = 1;
            funcDimension[funcNum] = 2;
            startingPoint[funcNum] = new double[funcDimension[funcNum]];
            startingPoint[funcNum][0] = 0;
            startingPoint[funcNum][1] = 0;
            exactSolution[funcNum] = new double[funcDimension[funcNum]];
            exactSolution[funcNum][0] = 1;
            exactSolution[funcNum][1] = 1;
            funcs[funcNum] = function1;
            funcsStr[funcNum] = "(x[1] - x[0] * x[0]) * (x[1] - x[0] * x[0]) + (1 - x[0]) * (1 - x[0])";

            ManyVariable function2 = delegate(double[] x)
            {
                return ((x[0] + 1) * (x[0] + 1) + x[0] * x[0]) * (x[0] * x[0] + (x[1] - 1) * (x[1] - 1));
            };
            funcNum = 2;
            funcDimension[funcNum] = 2;
            startingPoint[funcNum] = new double[funcDimension[funcNum]];
            startingPoint[funcNum][0] = 0.5;
            startingPoint[funcNum][1] = 0;
            exactSolution[funcNum] = new double[funcDimension[funcNum]];
            exactSolution[funcNum][0] = 0;
            exactSolution[funcNum][1] = 1;
            funcs[funcNum] = function2;
            funcsStr[funcNum] = "((x[0] + 1) * (x[0] + 1) + x[0] * x[0]) * (x[0] * x[0] + (x[1] - 1) * (x[1] - 1))";

            ManyVariable function3 = delegate(double[] x)
            {
                return (x[1] * x[1] + x[0] * x[0] - 1) * (x[1] * x[1] + x[0] * x[0] - 1) +
                    (x[0] + x[1] - 1) * (x[0] + x[1] - 1);
            };
            funcNum = 3;
            funcDimension[funcNum] = 2;
            startingPoint[funcNum] = new double[funcDimension[funcNum]];
            startingPoint[funcNum][0] = 0;
            startingPoint[funcNum][1] = 3;
            exactSolution[funcNum] = new double[funcDimension[funcNum]];
            exactSolution[funcNum][0] = 0;
            exactSolution[funcNum][1] = 1;
            funcs[funcNum] = function3;
            funcsStr[funcNum] = "(x[1] * x[1] + x[0] * x[0] - 1) * (x[1] * x[1] + x[0] * x[0] - 1) + (x[0] + x[1] - 1) * (x[0] + x[1] - 1)";

            ManyVariable function4 = delegate(double[] x)
            {
                return -x[0] * x[0] * System.Math.Exp(1 - x[0] * x[0] - 20.25 * (x[0] - x[1]) * (x[0] - x[1]));
            };
            funcNum = 4;
            funcDimension[funcNum] = 2;
            startingPoint[funcNum] = new double[funcDimension[funcNum]];
            startingPoint[funcNum][0] = 0.1;
            startingPoint[funcNum][1] = 0.5;
            exactSolution[funcNum] = new double[funcDimension[funcNum]];
            exactSolution[funcNum][0] = 1;
            exactSolution[funcNum][1] = 1;
            funcs[funcNum] = function4;
            funcsStr[funcNum] = "-x[0] * x[0] * System.Math.Exp(1 - x[0] * x[0] - 20.25 * (x[0] - x[1]) * (x[0] - x[1]))";

            ManyVariable function5 = delegate(double[] x)
            {
                return -x[0] * x[1] * System.Math.Exp(-(x[0] + x[1]));
            };
            funcNum = 5;
            funcDimension[funcNum] = 2;
            startingPoint[funcNum] = new double[funcDimension[funcNum]];
            startingPoint[funcNum][0] = 0;
            startingPoint[funcNum][1] = 1;
            exactSolution[funcNum] = new double[funcDimension[funcNum]];
            exactSolution[funcNum][0] = 1;
            exactSolution[funcNum][1] = 1;
            funcs[funcNum] = function5;
            funcsStr[funcNum] = "-x[0] * x[1] * System.Math.Exp(-(x[0] + x[1]))";

            ManyVariable function6 = delegate(double[] x)
            {
                return 4 * (x[0] - 5) * (x[0] - 5) + (x[1] - 6) * (x[1] - 6);
            };
            funcNum = 6;
            funcDimension[funcNum] = 2;
            startingPoint[funcNum] = new double[funcDimension[funcNum]];
            startingPoint[funcNum][0] = 8;
            startingPoint[funcNum][1] = 9;
            exactSolution[funcNum] = new double[funcDimension[funcNum]];
            exactSolution[funcNum][0] = 5;
            exactSolution[funcNum][1] = 6;
            funcs[funcNum] = function6;
            funcsStr[funcNum] = "4 * (x[0] - 5) * (x[0] - 5) + (x[1] - 6) * (x[1] - 6)";

            ManyVariable function7 = delegate(double[] x)
            {
                return (x[0] * x[0] + x[1] - 11) * (x[0] * x[0] + x[1] - 11) + (x[0] + x[1] * x[1] - 7) * (x[0] + x[1] * x[1] - 7);
            };
            funcNum = 7;
            funcDimension[funcNum] = 2;
            startingPoint[funcNum] = new double[funcDimension[funcNum]];
            startingPoint[funcNum][0] = 0;
            startingPoint[funcNum][1] = 0;
            exactSolution[funcNum] = new double[funcDimension[funcNum]];
            exactSolution[funcNum][0] = 3;
            exactSolution[funcNum][1] = 2;
            funcs[funcNum] = function7;
            funcsStr[funcNum] = "(x[0] * x[0] + x[1] - 11) * (x[0] * x[0] + x[1] - 11) + (x[0] + x[1] * x[1] - 7) * (x[0] + x[1] * x[1] - 7)";

            ManyVariable function8 = delegate(double[] x)
            {
                return 2 * x[0] * x[0] + x[0] * x[1] + x[1] * x[1];
            };
            funcNum = 8;
            funcDimension[funcNum] = 2;
            startingPoint[funcNum] = new double[funcDimension[funcNum]];
            startingPoint[funcNum][0] = 0.5;
            startingPoint[funcNum][1] = 1;
            exactSolution[funcNum] = new double[funcDimension[funcNum]];
            exactSolution[funcNum][0] = 0;
            exactSolution[funcNum][1] = 0;
            funcs[funcNum] = function8;
            funcsStr[funcNum] = "2 * x[0] * x[0] + x[0] * x[1] + x[1] * x[1]";

            ManyVariable function9 = delegate(double[] x)
            {
                return x[0] * x[0] * x[0] + x[1] * x[1] - 3 * x[0] - 2 * x[1] + 1;
            };
            funcNum = 9;
            funcDimension[funcNum] = 2;
            startingPoint[funcNum] = new double[funcDimension[funcNum]];
            startingPoint[funcNum][0] = 0.5;
            startingPoint[funcNum][1] = 1;
            exactSolution[funcNum] = new double[funcDimension[funcNum]];
            exactSolution[funcNum][0] = 1;
            exactSolution[funcNum][1] = 1;
            funcs[funcNum] = function9;
            funcsStr[funcNum] = "x[0] * x[0] * x[0] + x[1] * x[1] - 3 * x[0] - 2 * x[1] + 1";

            ManyVariable function10 = delegate(double[] x)
            {
                return x[0] * x[0] * x[0] * x[0] + x[1] * x[1] * x[1] * x[1] +
                    2 * x[0] * x[0] * x[1] * x[1] - 4 * x[0] + 3;
            };
            funcNum = 10;
            funcDimension[funcNum] = 2;
            startingPoint[funcNum] = new double[funcDimension[funcNum]];
            startingPoint[funcNum][0] = 0.5;
            startingPoint[funcNum][1] = 1;
            exactSolution[funcNum] = new double[funcDimension[funcNum]];
            exactSolution[funcNum][0] = 1;
            exactSolution[funcNum][1] = 0;
            funcs[funcNum] = function10;
            funcsStr[funcNum] = "x[0] * x[0] * x[0] * x[0] + x[1] * x[1] * x[1] * x[1] + 2 * x[0] * x[0] * x[1] * x[1] - 4 * x[0] + 3";
        }
    }
}