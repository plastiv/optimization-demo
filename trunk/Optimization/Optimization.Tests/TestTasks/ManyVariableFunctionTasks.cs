namespace Optimization.Tests.Tasks
{
    using System;
    using Optimization.Methods;

    abstract public class ManyVariableFunctionTask
    {
        public ManyVariable function;
        public int funcDimension;
        public double[] startPoint;
        public double[] exactSolution;
    }

    public class ManyVariableFunctionTask0 : ManyVariableFunctionTask
    {
        public ManyVariableFunctionTask0()
        {
            this.function = delegate(double[] x)
                        {
                            return x[0] * x[0] * x[0] - x[0] * x[1] + x[1] * x[1] - 2 * x[0] + 3 * x[1] - 4;
                        };
            this.startPoint = new double[] { 0, 0 };
            this.exactSolution = new double[] { 0.5, -1.25 };
            this.funcDimension = 2;
        }

        public override string ToString()
        {
            return "Function 0";
        }
    }

    public class ManyVariableFunctionTask1 : ManyVariableFunctionTask
    {
        public ManyVariableFunctionTask1()
        {
            this.function = delegate(double[] x)
            {
                return (x[1] - x[0] * x[0]) * (x[1] - x[0] * x[0]) + (1 - x[0]) * (1 - x[0]);
            };
            this.startPoint = new double[] { 0, 0 };
            this.exactSolution = new double[] { 1, 1 };
            this.funcDimension = 2;
        }

        public override string ToString()
        {
            return "Function 1";
        }
    }

    public class ManyVariableFunctionTask2 : ManyVariableFunctionTask
    {
        public ManyVariableFunctionTask2()
        {
            this.function = delegate(double[] x)
                    {
                        return ((x[0] + 1) * (x[0] + 1) + x[0] * x[0]) * (x[0] * x[0] + (x[1] - 1) * (x[1] - 1));
                    };
                    this.startPoint = new double[] { 0.5, 0 };
                    this.exactSolution = new double[] { 0, 1 };
            this.funcDimension = 2;
        }

        public override string ToString()
        {
            return "Function 2";
        }
    }

    public class ManyVariableFunctionTask3 : ManyVariableFunctionTask
    {
        public ManyVariableFunctionTask3()
        {
            this.function = delegate(double[] x)
                    {
                        return (x[1] * x[1] + x[0] * x[0] - 1) * (x[1] * x[1] + x[0] * x[0] - 1) +
                            (x[0] + x[1] - 1) * (x[0] + x[1] - 1);
                    };
                    this.startPoint = new double[] { 0, 3 };
                    this.exactSolution = new double[] { 0, 1 };
            this.funcDimension = 2;
        }

        public override string ToString()
        {
            return "Function 3";
        }
    }

    public class ManyVariableFunctionTask4 : ManyVariableFunctionTask
    {
        public ManyVariableFunctionTask4()
        {
            this.function = delegate(double[] x)
                    {
                        return -x[0] * x[0] * Math.Exp(1 - x[0] * x[0] - 20.25 * (x[0] - x[1]) * (x[0] - x[1]));
                    };
                    this.startPoint = new double[] { 0.1, 0.5 };
                    this.exactSolution = new double[] { 1, 1 };
            this.funcDimension = 2;
        }

        public override string ToString()
        {
            return "Function 4";
        }
    }

    public class ManyVariableFunctionTask5 : ManyVariableFunctionTask
    {
        public ManyVariableFunctionTask5()
        {
            this.function = delegate(double[] x)
                    {
                        return -x[0] * x[1] * Math.Exp(-(x[0] + x[1]));
                    };
                    this.startPoint = new double[] { 0, 1 };
                    this.exactSolution = new double[] { 1, 1 };
            this.funcDimension = 2;
        }

        public override string ToString()
        {
            return "Function 5";
        }
    }

    public class ManyVariableFunctionTask6 : ManyVariableFunctionTask
    {
        public ManyVariableFunctionTask6()
        {
            this.function = delegate(double[] x)
                    {
                        return 4 * (x[0] - 5) * (x[0] - 5) + (x[1] - 6) * (x[1] - 6);
                    };
                    this.startPoint = new double[] { 8, 9 };
                    this.exactSolution = new double[] { 5, 6 };
            this.funcDimension = 2;
        }

        public override string ToString()
        {
            return "Function 6";
        }
    }

    public class ManyVariableFunctionTask7 : ManyVariableFunctionTask
    {
        public ManyVariableFunctionTask7()
        {
            this.function = delegate(double[] x)
                    {
                        return (x[0] * x[0] + x[1] - 11) * (x[0] * x[0] + x[1] - 11) + (x[0] + x[1] * x[1] - 7) * (x[0] + x[1] * x[1] - 7);
                    };
                    this.startPoint = new double[] { 0, 0 };
                    this.exactSolution = new double[] { 3, 2 };
            this.funcDimension = 2;
        }

        public override string ToString()
        {
            return "Function 7";
        }
    }

    public class ManyVariableFunctionTask8 : ManyVariableFunctionTask
    {
        public ManyVariableFunctionTask8()
        {
            this.function = delegate(double[] x)
                    {
                        return 2 * x[0] * x[0] + x[0] * x[1] + x[1] * x[1];
                    };
                    this.startPoint = new double[] { 0.5, 1 };
                    this.exactSolution = new double[] { 0, 0 };
            this.funcDimension = 2;
        }

        public override string ToString()
        {
            return "Function 8";
        }
    }

    public class ManyVariableFunctionTask9 : ManyVariableFunctionTask
    {
        public ManyVariableFunctionTask9()
        {
            this.function = delegate(double[] x)
                    {
                        return x[0] * x[0] * x[0] + x[1] * x[1] - 3 * x[0] - 2 * x[1] + 1;
                    };
                    this.startPoint = new double[] { 0.5, 1 };
                    this.exactSolution = new double[] { 1, 1 };
            this.funcDimension = 2;
        }

        public override string ToString()
        {
            return "Function 9";
        }
    }

    public class ManyVariableFunctionTask10 : ManyVariableFunctionTask
    {
        public ManyVariableFunctionTask10()
        {
            this.function = delegate(double[] x)
            {
                return x[0] * x[0] * x[0] * x[0] + x[1] * x[1] * x[1] * x[1] +
                    2 * x[0] * x[0] * x[1] * x[1] - 4 * x[0] + 3;
            };
            this.startPoint = new double[] { 0.5, 1 };
            this.exactSolution = new double[] { 1, 0 };
            this.funcDimension = 2;
        }

        public override string ToString()
        {
            return "Function 10";
        }
    }
}