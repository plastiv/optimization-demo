//-----------------------------------------------------------------------
// <copyright file="Hooke-JeveesExtended.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace Optimization.Methods.ZerothOrder
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Optimization.Methods;

    /// <summary>
    /// Нахождение безусловного минимума функции многих переменных методом Хука-Дживса
    /// </summary>
    internal class Hooke_JeveesExtended : Hooke_Jevees
    {
        #region Private Fields

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Hooke_JeveesExtended"/> class.
        /// </summary>
        /// <param name="inputFunc">The input function.</param>
        /// <param name="funcDimension">The func dimension.</param>
        public Hooke_JeveesExtended(ManyVariable inputFunc, int funcDimension)
            : base(inputFunc, funcDimension)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hooke_JeveesExtended"/> class.
        /// </summary>
        /// <param name="inputFunc">The input function.</param>
        /// <param name="dimension">The dimension.</param>
        /// <param name="accelerateCoefficient">The accelerate coefficient.</param>
        /// <param name="coefficientReduction">The coefficient reduction.</param>
        /// <param name="step">The step value.</param>
        public Hooke_JeveesExtended(ManyVariable inputFunc, int dimension, double accelerateCoefficient, double coefficientReduction, double[] step)
            : base(inputFunc, dimension, accelerateCoefficient, coefficientReduction, step)
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the minimum.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="precision">The precision.</param>
        /// <returns>
        /// Вектор значений х, при котором функция достигает минимума.
        /// </returns>
        internal new double[][] GetMinimum(double[] startPoint, double precision)
        {
            // Шаг 1. Задать начальную точку л:0
            // число е>0 для остановки алгоритма
            Debug.Assert(precision > 0, "Precision is unexepectedly less or equal zero");

            List<Point> listPoinns = new List<Point>();

            Point newBasis = new Point(startPoint);
            Point oldBasis = new Point(startPoint);

            while (true)
            {
                listPoinns.Add(new Point(oldBasis.ToDouble()));

                // Шаг 2. Осуществить исследующий поиск по выбранному координатному направлению (i)
                newBasis = this.ExploratarySearch(newBasis);

                // Проверить успешность исследующего поиска:
                if (this.GetFuncValue(newBasis) < this.GetFuncValue(oldBasis))
                {
                    // перейти к шагу 4;

                    // Сформируем х[k]
                    Point oldOldBasis = new Point(oldBasis.ToDouble());
                    oldBasis.SetEqual(newBasis);

                    // y[0] = x[k + 1] + AccelerateCoefficient * (x[k + 1] - x[k]);
                    newBasis = this.PatternSearch(oldOldBasis, oldBasis);

                    // перейти к шагу 2.
                    continue;
                }
                else
                {
                    // перейти к шагу 5.

                    // Шаг 5. Проверить условие окончания:
                    if (!this.AllStepsLessPrecision(precision))
                    {
                        for (int index = 0; index < this.Dimension; index++)
                        {
                            // Для значений шагов, больших точности
                            if (this.step[index] > precision)
                            {
                                // Уменьшить величину шага
                                this.step[index] /= this.CoefficientReduction;
                            }
                        }

                        newBasis.SetEqual(oldBasis);

                        // перейти к шагу 2.
                        continue;
                    }
                    else
                    {
                        // Значение всех шагов меньше точности
                        // Поиск закончен
                        return this.ConvertToDouble(listPoinns);
                    }
                }
            }
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