//-----------------------------------------------------------------------
// <copyright file="ComboPenalty.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace Optimization.Methods.ConditionalExtremum
{
    using System.Diagnostics;

    /// <summary>
    /// Нахождение условного минимума функции многих переменных методом комбинирования штрафных и барьерных функций
    /// </summary>
    public class ComboPenalty
    {
        #region Public Fields

        #endregion

        #region Private Fields
        /// <summary>
        /// Парметры метода
        /// </summary>
        private MethodParams param;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ComboPenalty"/> class.
        /// </summary>
        /// <param name="inputParam">The input param.</param>
        public ComboPenalty(MethodParams inputParam)
        {
            Debug.Assert(inputParam.Func != null, "Function is unexepectedly not create (null)");
            Debug.Assert(inputParam.IncPenalty > 1, "InputParam.incrementPenalty is unexepectedly less or equal 1");
            Debug.Assert(inputParam.Penalty > 0, "Penalty is unexepectedly less or equal 0");
            Debug.Assert(inputParam.QuantityOfEqualities >= 0, "QuantityOfEqualities is unexepectedly less 0");
            Debug.Assert(inputParam.QuantityOfInequalities >= 0, "QuantityOfInequalities is unexepectedly less 0");

            this.param = inputParam;
        }
        #endregion

        #region Properties

        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the minimum.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="precision">Малое число е &gt; 0 для остановки алгоритма</param>
        /// <returns>Условный минимум функции.</returns>
        public double[] GetMinimum(double[] startPoint, double precision)
        {
            Debug.Assert(precision > 0, "Precision is unexepectedly less or equal zero");

            // Шаг 2. Составить вспомогательную функцию
            ManyVariable auxiliaryFunction = delegate(double[] inputx)
            {
                return this.param.Func(inputx) + this.PenaltyFunction(inputx, this.param.Penalty);
            };

            double[] xopt = Minimum.HookeJevees(this.param.Func, this.param.Dimension, startPoint); // искомая точка точка

            while (System.Math.Abs(this.PenaltyFunction(xopt, this.param.Penalty)) > precision)
            {
                this.Condition(xopt);
                xopt = Minimum.HookeJevees(auxiliaryFunction, this.param.Dimension, xopt);
                this.param.Penalty = this.param.Penalty / this.param.IncPenalty;
            }

            return xopt;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Conditions the specified x.
        /// </summary>
        /// <param name="x">Переменная x.</param>
        /// <returns>Правду, если выполняются условия gi(x) less zero.</returns>
        private bool Condition(double[] x)
        {
            if (this.param.QuantityOfInequalities != 0)
            {
                for (int i = 0; i < this.param.QuantityOfInequalities; i++)
                {
                    if (this.param.Inequalities[i](x) > 0)
                    {
                        // TODO: bad style
                        System.Console.WriteLine("Greater then zero");
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Штрафная функция
        /// </summary>
        /// <param name="x">Переменная x.</param>
        /// <returns>Значение штрафной функции.</returns>
        private double QuadraticPenalty(double[] x)
        {
            if (this.param.QuantityOfEqualities != 0)
            {
                double solution = 0;

                for (int i = 0; i < this.param.QuantityOfEqualities; i++)
                {
                    solution = solution + (this.param.Equalities[i](x) * this.param.Equalities[i](x));
                }

                return solution;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Барьерная функция
        /// </summary>
        /// <param name="x">Переменная x.</param>
        /// <returns>Значение барьерной функции.</returns>
        private double SquareShear(double[] x)
        {
            if (this.param.QuantityOfInequalities != 0)
            {
                double solution = 0;

                for (int i = 0; i < this.param.QuantityOfInequalities; i++)
                {
                    solution = solution + (1 / this.param.Inequalities[i](x));
                }

                return solution;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Penalties the function.
        /// </summary>
        /// <param name="x">Переменная x.</param>
        /// <param name="r">Параметр штрафа.</param>
        /// <returns>Значение общей штрафной функции.</returns>
        private double PenaltyFunction(double[] x, double r)
        {
            return (this.QuadraticPenalty(x) / (2 * r)) - (r * this.SquareShear(x));
        }
        #endregion

        #region Structs
        /// <summary>
        /// Парметры метода
        /// </summary>
        public struct MethodParams
        {
            /// <summary>
            /// Количество неравенств
            /// </summary>
            public int QuantityOfInequalities;

            /// <summary>
            /// Ссылки на неравенства
            /// </summary>
            public ManyVariable[] Inequalities;

            /// <summary>
            /// Количество равенств
            /// </summary>
            public int QuantityOfEqualities;

            /// <summary>
            /// Ссылки на равенства
            /// </summary>
            public ManyVariable[] Equalities;

            /// <summary>
            /// Ссылка на функцию
            /// </summary>
            public ManyVariable Func;

            /// <summary>
            /// Начальное значение параметра штрафа r0 > 0
            /// </summary>
            public double Penalty;

            /// <summary>
            /// Число С > 1 для увеличения параметра
            /// </summary>
            public double IncPenalty;

            /// <summary>
            /// Количество переменных
            /// </summary>
            public int Dimension;
        }
        #endregion
    }
}
