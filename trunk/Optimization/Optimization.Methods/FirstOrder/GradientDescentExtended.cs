﻿//-----------------------------------------------------------------------
// <copyright file="GradientDescentExtended.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2010. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace Optimization.Methods.FirstOrder
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Optimization.Methods;

    /// <summary>
    /// Метод градиентного спуска с постоянным шагом
    /// </summary>
    internal class GradientDescentExtended
    {
        #region Private Fields
        /// <summary>
        /// Искомая функция.
        /// </summary>
        private readonly ManyVariable SearchFunc;

        /// <summary>
        /// Первая производная искомой функции.
        /// </summary>
        private readonly Gradient SearchGradient;

        /// <summary>
        /// Количество переменных в минимизируемом уравнении.
        /// </summary>
        private readonly int Dimension;

        /// <summary>
        /// Малое положительное число, ограничивающее евклидову норму градиента. 
        /// </summary>
        private readonly double Epsilon1;

        /// <summary>
        /// Малое положительное число, ограничивающее евклидову норму градиента. 
        /// </summary>
        private readonly double Epsilon2;

        /// <summary>
        /// Максимальное число итераций для метода.
        /// </summary>
        private readonly int IterationCount;

        /// <summary>
        /// Величина шага deltax.
        /// </summary>
        private double step;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GradientDescentExtended"/> class.
        /// </summary>
        /// <param name="searchFunc">The search func.</param>
        /// <param name="searchGradient">The search gradient.</param>
        /// <param name="dimension">The dimension.</param>
        /// <param name="iterationCount">The iteration count.</param>
        /// <param name="step">The step value.</param>
        /// <param name="epsilon1">The epsilon1.</param>
        /// <param name="epsilon2">The epsilon2.</param>
        public GradientDescentExtended(ManyVariable searchFunc, Gradient searchGradient, int dimension, int iterationCount, double step, double epsilon1, double epsilon2)
        {
            Debug.Assert(epsilon1 > 0, "Epsilon1 is unexepectedly less or equal zero");
            Debug.Assert(epsilon2 > 0, "Epsilon2 is unexepectedly less or equal zero");
            Debug.Assert(iterationCount > 0, "MaxIteration is unexepectedly less or equal zero");
            Debug.Assert(step > 0, "Step is unexepectedly less or equal zero");
            Debug.Assert(dimension > 1, "Dimension is unexepectedly less or equal 1");

            this.SearchFunc = searchFunc;
            this.SearchGradient = searchGradient;
            this.Dimension = dimension;
            this.IterationCount = iterationCount;
            this.step = step;
            this.Epsilon1 = epsilon1;
            this.Epsilon2 = epsilon2;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientDescentExtended"/> class.
        /// </summary>
        /// <param name="searchFunc">The search func.</param>
        /// <param name="dimension">The dimension.</param>
        /// <param name="iterationCount">The iteration count.</param>
        /// <param name="step">The step value.</param>
        /// <param name="epsilon1">The epsilon1.</param>
        /// <param name="epsilon2">The epsilon2.</param>
        public GradientDescentExtended(ManyVariable searchFunc, int dimension, int iterationCount, double step, double epsilon1, double epsilon2)
            : this(searchFunc, null, dimension, iterationCount, step, epsilon1, epsilon2)
        {
            this.SearchGradient = this.GetNumericalGradient;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientDescentExtended"/> class.
        /// </summary>
        /// <param name="searchFunc">The search func.</param>
        /// <param name="funcDimension">The func dimension.</param>
        public GradientDescentExtended(ManyVariable searchFunc, int funcDimension)
        {
            this.Dimension = funcDimension;
            this.Epsilon1 = 0.1;
            this.Epsilon2 = 0.15;
            this.IterationCount = 25;
            this.step = 2;
            this.SearchFunc = searchFunc;
            this.SearchGradient = this.GetNumericalGradient;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Нахождение безусловного локального минимума функции многих переменных.
        /// </summary>
        /// <param name="startPoint">Начальная точка.</param>
        /// <returns>Вектор значений х, при котором функция достигает минимума.</returns>
        internal double[][] GetMinimum(double[] startPoint)
        {
            List<Point> result = new List<Point>();
            Point currPoint = new Point(startPoint);
            Point prevPoint = new Point(this.Dimension);
            Point prevPrevPoint = new Point(this.Dimension);

            result.Add(currPoint);
            int iteration = 0;

            while (this.IsLessIterationCountAndGreaterEpsilon1(iteration, currPoint))
            {
                prevPrevPoint.SetEqual(prevPoint);
                prevPoint.SetEqual(currPoint);

                currPoint = this.GetNextPoint(prevPoint);
                result.Add(currPoint);

                if (this.IsLessEpsilon2(currPoint, prevPoint, prevPrevPoint))
                {
                    return this.ConvertToDouble(result);
                }
                else
                {
                    iteration++;
                }
            }

            return this.ConvertToDouble(result);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Converts to double.
        /// </summary>
        /// <param name="item">The List item.</param>
        /// <returns>Item in double.</returns>
        private double[][] ConvertToDouble(List<Point> item)
        {
            Point[] points = item.ToArray();
            double[][] result = new double[item.Count][];
            for (int i = 0; i < item.Count; i++)
            {
                result[i] = points[i].ToDouble();
            }

            return result;
        }

        /// <summary>
        /// Gets the func value.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>Значение функции в точке.</returns>
        private double GetFuncValue(Point point)
        {
            return this.SearchFunc(point.ToDouble());
        }

        /// <summary>
        /// Gets the gradient value.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>Значение градиента в точке.</returns>
        private double[] GetGradientValue(Point point)
        {
            return this.SearchGradient(point.ToDouble());
        }

        /// <summary>
        /// Determines whether [is less iteration count and greater epsilon1] [the specified iteration].
        /// </summary>
        /// <param name="iteration">The iteration.</param>
        /// <param name="curr">The current point.</param>
        /// <returns>
        /// <c>true</c> if [is less iteration count and greater epsilon1] [the specified iteration]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsLessIterationCountAndGreaterEpsilon1(int iteration, Point curr)
        {
            if (iteration < this.IterationCount && this.GetEuclideanNorm(this.GetGradientValue(curr)) > this.Epsilon1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether [is less epsilon2] [the specified curr].
        /// </summary>
        /// <param name="curr">The current point.</param>
        /// <param name="prev">The previous point.</param>
        /// <param name="prevPrev">The prev previous point.</param>
        /// <returns>
        /// <c>true</c> if [is less epsilon2] [the specified curr]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsLessEpsilon2(Point curr, Point prev, Point prevPrev)
        {
            if (this.IsLessEpsilon2(curr, prev) && this.IsLessEpsilon2(prev, prevPrev))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the specified point1 is condition.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <returns>
        /// <c>true</c> if the specified point1 is condition; otherwise, <c>false</c>.
        /// </returns>
        private bool IsLessEpsilon2(Point point1, Point point2)
        {
            if (this.GetEuclideanNorm((point1 - point2).ToDouble()) < this.Epsilon2)
            {
                if (Math.Abs(this.GetFuncValue(point1) - this.GetFuncValue(point2)) < this.Epsilon2)
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
            double[] solution = new double[this.Dimension];
            for (int i = 0; i < this.Dimension; i++)
            {
                double[] x1 = new double[this.Dimension];
                double[] x2 = new double[this.Dimension];
                for (int j = 0; j < this.Dimension; j++)
                {
                    x1[j] = x[j];
                    x2[j] = x[j];
                }

                x1[i] -= DeltaX;
                x2[i] += DeltaX;
                solution[i] = (this.SearchFunc(x2) - this.SearchFunc(x1)) / (2 * DeltaX);
            }

            return solution;
        }

        /// <summary>
        /// Получить значения х для следующей точки.
        /// </summary>
        /// <param name="previousPoint">Предыдущая точка.</param>
        /// <returns>Значения х.</returns>
        private Point GetNextPoint(Point previousPoint)
        {
            Point gradientValue = new Point(this.SearchGradient(previousPoint.ToDouble()));
            Point nextPoint = previousPoint - (this.step * gradientValue);

            while (this.GetFuncValue(nextPoint) - this.GetFuncValue(previousPoint) > 0)
            {
                this.step /= 2;
                nextPoint = previousPoint - (this.step * gradientValue);
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

            for (int i = 0; i < this.Dimension; i++)
            {
                eps += vector[i] * vector[i];
            }

            eps = Math.Sqrt(eps);
            return eps;
        }
        #endregion

        #region Structs

        #endregion
    }
}
