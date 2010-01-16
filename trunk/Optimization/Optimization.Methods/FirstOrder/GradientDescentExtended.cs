//-----------------------------------------------------------------------
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
    internal class GradientDescentExtended : GradientDescent
    {
        #region Private Fields

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
            : base(searchFunc, searchGradient, dimension, iterationCount, step, epsilon1, epsilon2)
        {
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
            : base(searchFunc, dimension, iterationCount, step, epsilon1, epsilon2)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientDescentExtended"/> class.
        /// </summary>
        /// <param name="searchFunc">The search func.</param>
        /// <param name="funcDimension">The func dimension.</param>
        public GradientDescentExtended(ManyVariable searchFunc, int funcDimension)
            : base(searchFunc, funcDimension)
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Нахождение безусловного локального минимума функции многих переменных.
        /// </summary>
        /// <param name="startPoint">Начальная точка.</param>
        /// <returns>Вектор значений х, при котором функция достигает минимума.</returns>
        internal new double[][] GetMinimum(double[] startPoint)
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
        #endregion

        #region Structs

        #endregion
    }
}
