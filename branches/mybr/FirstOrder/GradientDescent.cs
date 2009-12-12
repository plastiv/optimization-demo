//-----------------------------------------------------------------------
// <copyright file="GradientDescent.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace OptimizationMethods.FirstOrder
{
    using System.Diagnostics;
    using OptimizationMethods;

    /// <summary>
    /// Метод градиентного спуска с постоянным шагом
    /// </summary>
    public class GradientDescent
    {
        #region Private Fields
        /// <summary>
        /// Искомая функция.
        /// </summary>
        private readonly ManyVariable searchFunc;

        /// <summary>
        /// Первая производная искомой функции.
        /// </summary>
        private readonly Gradient searchGradient;

        /// <summary>
        /// Параметры метода.
        /// </summary>
        private readonly MethodParams param;

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
        public GradientDescent(ManyVariable searchFunc, Gradient searchGradient, MethodParams initParam)
        {
            Debug.Assert(initParam.Epsilon1 > 0, "Epsilon1 is unexepectedly less or equal zero");
            Debug.Assert(initParam.Epsilon2 > 0, "Epsilon2 is unexepectedly less or equal zero");
            Debug.Assert(initParam.IterationCount > 0, "MaxIteration is unexepectedly less or equal zero");
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
        public GradientDescent(ManyVariable searchFunc, MethodParams initParam)
            : this(searchFunc, null, initParam)
        {
            this.searchGradient = this.GetNumericalGradient;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientDescent"/> class.
        /// </summary>
        /// <param name="searchFunc">The search func.</param>
        /// <param name="funcDimension">The func dimension.</param>
        public GradientDescent(ManyVariable searchFunc, int funcDimension)
        {
            this.param.Dimension = funcDimension;
            this.param.Epsilon1 = 0.1;
            this.param.Epsilon2 = 0.15;
            this.param.IterationCount = 25;
            this.param.Step = 2;
            this.step = this.param.Step;
            this.searchFunc = searchFunc;
            this.searchGradient = this.GetNumericalGradient;
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
            double[] currPoint = new double[this.param.Dimension];
            for (int i = 0; i < this.param.Dimension; i++)
            {
                currPoint[i] = startPoint[i];
            }

            double[] prevPoint = new double[this.param.Dimension];
            double[] prevPrevPoint = new double[this.param.Dimension];

            int iteration = 0;

            while (iteration < this.param.IterationCount && this.GetEuclideanNorm(this.searchGradient(currPoint)) > this.param.Epsilon1)
            {
                for (int i = 0; i < this.param.Dimension; i++)
                {
                    prevPrevPoint[i] = prevPoint[i];
                    prevPoint[i] = currPoint[i];
                }

                currPoint = this.GetNextPoint(prevPoint);

                if (this.IsCondition(currPoint, prevPoint))
                {
                    if (this.IsCondition(prevPoint, prevPrevPoint))
                    {
                        return currPoint;
                    }
                }

                iteration++;
            }

            return currPoint;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Determines whether the specified point1 is condition.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <returns>
        /// <c>true</c> if the specified point1 is condition; otherwise, <c>false</c>.
        /// </returns>
        private bool IsCondition(double[] point1, double[] point2)
        {
            double[] difference = new double[this.param.Dimension];
            for (int i = 0; i < this.param.Dimension; i++)
            {
                difference[i] = point1[i] - point2[i];
            }

            if (this.GetEuclideanNorm(difference) < this.param.Epsilon2)
            {
                if (System.Math.Abs(this.searchFunc(point1) - this.searchFunc(point2)) < this.param.Epsilon2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the numerical gradient.
        /// </summary>
        /// <param name="x">Точка x для ктр рассчитывается градиент.</param>
        /// <returns>Градиент функции.</returns>
        private double[] GetNumericalGradient(double[] x)
        {
            const double DeltaX = 0.00001;
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

                x1[i] -= DeltaX;
                x2[i] += DeltaX;
                solution[i] = (this.searchFunc(x2) - this.searchFunc(x1)) / (2 * DeltaX);
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
            double[] gradientValue = this.searchGradient(previousPoint);
            double[] nextPoint = new double[this.param.Dimension];

            for (int i = 0; i < this.param.Dimension; i++)
            {
                nextPoint[i] = previousPoint[i] - (this.step * gradientValue[i]);
            }

            while (this.searchFunc(nextPoint) - this.searchFunc(previousPoint) > 0)
            {
                this.step /= 2;

                for (int i = 0; i < this.param.Dimension; i++)
                {
                    nextPoint[i] = previousPoint[i] - (this.step * gradientValue[i]);
                }
            }

            return nextPoint;
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
            public int IterationCount;
        }
        #endregion
    }
}
