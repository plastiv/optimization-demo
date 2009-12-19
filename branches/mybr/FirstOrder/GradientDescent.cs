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
        private readonly Gradient SearchGradient;

        /// <summary>
        /// Параметры метода.
        /// </summary>
        private readonly MethodParams Param;

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
            this.Param = initParam;
            this.step = initParam.Step;
            this.searchFunc = searchFunc;
            this.SearchGradient = searchGradient;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientDescent"/> class.
        /// </summary>
        /// <param name="searchFunc">The search func.</param>
        /// <param name="initParam">The initial parameters.</param>
        public GradientDescent(ManyVariable searchFunc, MethodParams initParam)
            : this(searchFunc, null, initParam)
        {
            this.SearchGradient = this.GetNumericalGradient;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientDescent"/> class.
        /// </summary>
        /// <param name="searchFunc">The search func.</param>
        /// <param name="funcDimension">The func dimension.</param>
        public GradientDescent(ManyVariable searchFunc, int funcDimension)
        {
            this.Param.Dimension = funcDimension;
            this.Param.Epsilon1 = 0.1;
            this.Param.Epsilon2 = 0.15;
            this.Param.IterationCount = 25;
            this.Param.Step = 2;
            this.step = this.Param.Step;
            this.searchFunc = searchFunc;
            this.SearchGradient = this.GetNumericalGradient;
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
            double[] currPoint = new double[this.Param.Dimension];
            for (int i = 0; i < this.Param.Dimension; i++)
            {
                currPoint[i] = startPoint[i];
            }

            double[] prevPoint = new double[this.Param.Dimension];
            double[] prevPrevPoint = new double[this.Param.Dimension];

            int iteration = 0;

            while (iteration < this.Param.IterationCount && this.GetEuclideanNorm(this.SearchGradient(currPoint)) > this.Param.Epsilon1)
            {
                for (int i = 0; i < this.Param.Dimension; i++)
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

        /// <summary>
        /// Нахождение безусловного локального минимума функции многих переменных (с сохранением промежуточных пошаговых результатов).
        /// </summary>
        /// <param name="startPoint">Начальная точка.</param>
        /// <returns>Матрицу значений х, где первый мтератог - номер шага, второй итератор - указывает переменную, при котором функция достигает минимума.</returns>
        public double[][] GetExtendedMinimum(double[] startPoint)
        {
            int iteration = 0;
            double[][] currPoint = new double[this.Param.IterationCount][];
            for (int i = 0; i < this.Param.IterationCount; i++)
            {
                currPoint[i] = new double[this.Param.Dimension];
                for (int j = 0; j < this.Param.Dimension; j++)
                {
                    currPoint[i][j] = startPoint[j];
                }
            }

            double[] prevPoint = new double[this.Param.Dimension];
            double[] prevPrevPoint = new double[this.Param.Dimension];

            while (iteration < this.Param.IterationCount && this.GetEuclideanNorm(this.SearchGradient(currPoint[iteration])) > this.Param.Epsilon1)
            {
                for (int i = 0; i < this.Param.Dimension; i++)
                {
                    prevPrevPoint[i] = prevPoint[i];
                    prevPoint[i] = currPoint[iteration][i];
                }

                currPoint[iteration] = this.GetNextPoint(prevPoint);

                if (this.IsCondition(currPoint[iteration], prevPoint))
                {
                    if (this.IsCondition(prevPoint, prevPrevPoint))
                    {
                        return currPoint;
                    }
                }

                iteration++;
            }

            double[][] solution = new double[iteration + 1][];
            for (int i = 0; i < iteration + 1; i++)
            {
                solution[i] = new double[this.Param.Dimension];
                for (int j = 0; j < this.Param.Dimension; j++)
                {
                    solution[i][j] = currPoint[i][j];
                }
            }

            return solution;
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
            double[] difference = new double[this.Param.Dimension];
            for (int i = 0; i < this.Param.Dimension; i++)
            {
                difference[i] = point1[i] - point2[i];
            }

            if (this.GetEuclideanNorm(difference) < this.Param.Epsilon2)
            {
                if (System.Math.Abs(this.searchFunc(point1) - this.searchFunc(point2)) < this.Param.Epsilon2)
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
            double[] solution = new double[this.Param.Dimension];
            for (int i = 0; i < this.Param.Dimension; i++)
            {
                double[] x1 = new double[this.Param.Dimension];
                double[] x2 = new double[this.Param.Dimension];
                for (int j = 0; j < this.Param.Dimension; j++)
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
            double[] gradientValue = this.SearchGradient(previousPoint);
            double[] nextPoint = new double[this.Param.Dimension];

            for (int i = 0; i < this.Param.Dimension; i++)
            {
                nextPoint[i] = previousPoint[i] - (this.step * gradientValue[i]);
            }

            while (this.searchFunc(nextPoint) - this.searchFunc(previousPoint) > 0)
            {
                this.step /= 2;

                for (int i = 0; i < this.Param.Dimension; i++)
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

            for (int i = 0; i < this.Param.Dimension; i++)
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
