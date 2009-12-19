//-----------------------------------------------------------------------
// <copyright file="PenaltyAndBarrier.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace OptimizationMethods
{
    using OptimizationMethods.ConditionalExtremum;

    /// <summary>
    /// Нахождение безусловного минимума функции многих переменных
    /// </summary>
    public class ConditionalMinimum
    {
        /// <summary>
        /// Малое число для остановки алгоритма.
        /// </summary>
        private const double Precision = 0.01;

        /// <summary>
        /// Gradients the descent.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="equalities">The equalities.</param>
        /// <param name="inequalities">The inequalities.</param>
        /// <param name="quantityOfEqualities">The quantity of equalities.</param>
        /// <param name="quantityOfInequalities">The quantity of inequalities.</param>
        /// <param name="funcDimension">The func dimension.</param>
        /// <param name="startingPoint">The starting point.</param>
        /// <returns>Точку, при которой функция достигает минимума.</returns>
        public static double[] Penalty(ManyVariable function, ManyVariable[] equalities, ManyVariable[] inequalities, int quantityOfEqualities, int quantityOfInequalities, int funcDimension, double[] startingPoint)
        {
            Penalty.MethodParams param = new Penalty.MethodParams();
            param.Dimension = funcDimension;
            param.Equalities = equalities;
            param.Func = function;
            param.IncPenalty = 4;
            param.Inequalities = inequalities;
            param.Penalty = 1;
            param.QuantityOfEqualities = quantityOfEqualities;
            param.QuantityOfInequalities = quantityOfInequalities;

            Penalty penalty = new Penalty(param);
            return penalty.GetMinimum(startingPoint, Precision);
        }

        /// <summary>
        /// Comboes the penalty.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="equalities">The equalities.</param>
        /// <param name="inequalities">The inequalities.</param>
        /// <param name="quantityOfEqualities">The quantity of equalities.</param>
        /// <param name="quantityOfInequalities">The quantity of inequalities.</param>
        /// <param name="funcDimension">The func dimension.</param>
        /// <param name="startingPoint">The starting point.</param>
        /// <returns>Точку, при которой функция достигает минимума.</returns>
        public static double[] ComboPenalty(ManyVariable function, ManyVariable[] equalities, ManyVariable[] inequalities, int quantityOfEqualities, int quantityOfInequalities, int funcDimension, double[] startingPoint)
        {
            ComboPenalty.MethodParams param = new ComboPenalty.MethodParams();
            param.Dimension = funcDimension;
            param.Equalities = equalities;
            param.Func = function;
            param.IncPenalty = 4;
            param.Inequalities = inequalities;
            param.Penalty = 10;
            param.QuantityOfEqualities = quantityOfEqualities;
            param.QuantityOfInequalities = quantityOfInequalities;

            ComboPenalty comboPenalty = new ComboPenalty(param);
            return comboPenalty.GetMinimum(startingPoint, Precision);
        }
    }
}