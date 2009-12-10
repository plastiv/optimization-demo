//-----------------------------------------------------------------------
// <copyright file="GradientDescent.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace OptimizationMethods.FirstOrder
{
    using System.Diagnostics;

    /// <summary>
    /// Ссылка на функциональную зависимость f(x1,х2,...,хn).
    /// </summary>
    /// <param name="x">Вектор значений переменных x1,х2,...,хn.</param>
    /// <returns>Значение функции у=f(x1,х2,...,хn).</returns>
    public delegate double GetManyVariableFunctionValue(double[] x);

    /// <summary>
    /// Ссылка на функциональную зависимость df/dxi, где xi = x1,х2,...,хn.
    /// </summary>
    /// <param name="x">Вектор значений переменных x1,х2,...,хn.</param>
    /// <returns>Значение градиента у=df/dxi, где xi = x1,х2,...,хn.</returns>
    public delegate double[] GetGradientOfFunction(double[] x);

    /// <summary>
    /// Метод градиентного спуска с постоянным шагом
    /// </summary>
    public class GradientDescent
    {
        #region Private Fields
        private const double deltaX = 0.00001;

        /// <summary>
        /// Искомая функция.
        /// </summary>
        private readonly GetManyVariableFunctionValue searchFunc;

        /// <summary>
        /// Первая производная искомой функции.
        /// </summary>
        private readonly GetGradientOfFunction searchGradient;

        private readonly MethodParams param;

        /// <summary>
        /// Значение градиента в текущей точке.
        /// </summary>
        private double[] gradientValue;

        /// <summary>
        /// Величина шага deltax.
        /// </summary>
        private double step;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GradientDescent"/> class.
        /// </summary>
        /// <param name="searchFunc">The search func.</param>
        /// <param name="searchGradient">The search gradient.</param>
        /// <param name="initParam">The initial parameters.</param>
        public GradientDescent(GetManyVariableFunctionValue searchFunc, GetGradientOfFunction searchGradient,MethodParams initParam)
        {
            Debug.Assert(initParam.Epsilon1 > 0, "Epsilon1 is unexepectedly less or equal zero");
            Debug.Assert(initParam.Epsilon2 > 0, "Epsilon2 is unexepectedly less or equal zero");
            Debug.Assert(initParam.MaxIteration > 0, "MaxIteration is unexepectedly less or equal zero");
            Debug.Assert(initParam.Step > 0, "Step is unexepectedly less or equal zero");
            Debug.Assert(initParam.Dimension > 1, "Dimension is unexepectedly less or equal 1");
            this.param = initParam;
            this.step = initParam.Step;
            this.searchFunc = searchFunc;
            this.searchGradient = searchGradient;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientDescent"/> class.
        /// </summary>
        /// <param name="searchFunc">The search func.</param>
        /// <param name="initParam">The initial parameters.</param>
        public GradientDescent(GetManyVariableFunctionValue searchFunc, MethodParams initParam)
            : this(searchFunc, null, initParam)
        {
            searchGradient = GetNumericalGradient;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientDescent"/> class.
        /// </summary>
        /// <param name="searchFunc">The search func.</param>
        /// <param name="funcDimension">The func dimension.</param>
        public GradientDescent(GetManyVariableFunctionValue searchFunc, int funcDimension)
        {
            this.param.Dimension = funcDimension;
            this.param.Epsilon1 = 0.1;
            this.param.Epsilon2 = 0.15;
            this.param.MaxIteration = 25;
            this.param.Step = 2;
            this.step = param.Step;
            this.searchFunc = searchFunc;
            this.searchGradient = GetNumericalGradient;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Нахождение безусловного локального минимума функции многих переменных.
        /// </summary>
        /// <param name="startPoint">Начальная точка.</param>
        /// <returns>Вектор значений х, при котором функция достигает минимума.</returns>
        public double[] GetMinimum(double[] startPoint)
        {
            double[] currPoint = startPoint;
            double[] prevPoint = startPoint;
            double[] prevPrevPoint = startPoint;

            this.gradientValue = this.searchGradient(startPoint);

            int count = 0;
            while (count < this.param.MaxIteration && this.GetEuclideanNorm(this.gradientValue) > this.param.Epsilon1)
            {
                count++;
                prevPrevPoint = prevPoint;
                prevPoint = currPoint;
                currPoint = this.GetNextPoint(prevPoint);
                if (this.GetEuclideanNorm(currPoint, prevPoint) < this.param.Epsilon2
                    && this.searchFunc(currPoint) - this.searchFunc(prevPoint) < this.param.Epsilon2)
                {
                    if (this.GetEuclideanNorm(prevPoint, prevPrevPoint) < this.param.Epsilon2
                        && this.searchFunc(prevPoint) - this.searchFunc(prevPrevPoint) < this.param.Epsilon2)
                    {
                        return currPoint;
                    }
                }
            }

            return currPoint;
        }
        #endregion

        #region Private Methods

        private double[] GetNumericalGradient(double[] x)
        {
            double[] solution = new double[this.param.Dimension];
            for (int i = 0; i < this.param.Dimension; i++)
            {
                double[] x1 = new double[this.param.Dimension];
                double[] x2 = new double[this.param.Dimension];
                for (int j = 0; j < this.param.Dimension; j++)
                {
                    x1[j] = x[j];
                    x2[j] = x[j];
                }
                x1[i] -= deltaX;
                x2[i] += deltaX;
                solution[i] = (searchFunc(x2) - searchFunc(x1)) / (2 * deltaX);
            }

            return solution;
        }
        /// <summary>
        /// Получить значения х для следующей точки.
        /// </summary>
        /// <param name="previousPoint">Предыдущая точка.</param>
        /// <returns>Значения х.</returns>
        private double[] GetNextPoint(double[] previousPoint)
        {
            double[] refPoint = new double[this.param.Dimension];
            this.gradientValue = this.searchGradient(previousPoint);

            while (true)
            {
                for (int i = 0; i < this.param.Dimension; i++)
                {
                    refPoint[i] = previousPoint[i] - (this.step * this.gradientValue[i]);
                }

                if (this.searchFunc(refPoint) - this.searchFunc(previousPoint) <= 0)
                {
                    return refPoint;
                }

                this.step /= 2;
            }
        }

        /// <summary>
        /// Получить евклидову норму разности двух векторов.
        /// </summary>
        /// <param name="vector1">The vector1.</param>
        /// <param name="vector2">The vector2.</param>
        /// <returns>Евклидова норма вектора.</returns>
        private double GetEuclideanNorm(double[] vector1, double[] vector2)
        {
            double eps = 0;

            for (int i = 0; i < this.param.Dimension; i++)
            {
                eps += System.Math.Abs(vector1[i] - vector2[i]) * System.Math.Abs(vector1[i] - vector2[i]);
            }

            eps = System.Math.Sqrt(eps);
            return eps;
        }

        /// <summary>
        /// Получить евклидову норму вектора.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <returns>Евклидова норма вектора.</returns>
        private double GetEuclideanNorm(double[] vector)
        {
            double eps = 0;

            for (int i = 0; i < this.param.Dimension; i++)
            {
                eps += vector[i] * vector[i];
            }

            eps = System.Math.Sqrt(eps);
            return eps;
        }
        #endregion

        #region Structs
        /// <summary>
        /// Передаваемые в метод параметры.
        /// </summary>
        public struct MethodParams
        {
            /// <summary>
            /// Количество переменных в минимизируемом уравнении.
            /// </summary>
            public int Dimension;

            /// <summary>
            /// Малое положительное число, ограничивающее евклидову норму градиента. 
            /// </summary>
            public double Epsilon1;

            /// <summary>
            /// Малое положительное число, ограничивающее евклидову норму градиента. 
            /// </summary>
            public double Epsilon2;

            /// <summary>
            /// Величина шага deltax.
            /// </summary>
            public double Step;

            /// <summary>
            /// Максимальное число итераций для метода.
            /// </summary>
            public int MaxIteration;
        }
        #endregion
    }
}
