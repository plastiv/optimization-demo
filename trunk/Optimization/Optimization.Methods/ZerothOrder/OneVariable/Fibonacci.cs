//-----------------------------------------------------------------------
// <copyright file="Fibonacci.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace OptimizationMethods.ZerothOrder.OneVariable
{
    /// <summary>
    /// Оптимизация унимодальной функции одной переменной методом Фибоначчи
    /// </summary>
    /// ------------------------------------------------------------------------------------
    /// Метод относится к последовательным стратегиям. Задается начальный 
    /// интервал непределенности и количество N вычислений функции. Алгоритм 
    /// уменьшения интервала опирается на анализ значений функции в двух точках.
    /// Точки вычисления функции находятся с использованием 
    /// последовательности из N + 1 чисел Фибоначчи. Как в методе золотого сечения, на первой
    /// итерации требуются два вычисления функции, а на каждой последующей - 
    /// только по одному. Условия окончания процесса поиска стандартные: поиск 
    /// заканчивается, когда длина текущего интервала неопределенности оказывается меньше
    /// установленной величины.
    /// -------------------------------------------------------------------------------------
    public static class Fibonacci
    {
        #region Private Member

        /// <summary>
        /// Константа различимости
        /// </summary>
        private const double Epsilon = 0.001;

        /// <summary>
        /// Последовательность чисел Фибоначчи
        /// </summary>
        private static readonly int[] fibonacciSequence = 
        {
        1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 600, 977, 1577, 2554, 4131, 6685, 10816,
        17501, 28582, 46083, 74665, 120748, 195413, 316161, 511574 
        };

        #endregion

        #region Public Methods

        /// <summary>
        /// Нахождение безусловного минимума функции f(x) одной переменной.
        /// </summary>
        /// <param name="func">Функция f(x) одной переменной.</param>
        /// <param name="leftBound">Левая граница начального интервала неопределенности (a0).</param>
        /// <param name="rightBound">Правая граница начального интервала неопределенности (b0).</param>
        /// <param name="precision">Длина конечного интервала неопределенности (точность вычисления).</param>
        /// <returns>Безусловный минимум функции (x_min)</returns>
        public static double GetMinimum(OneVariableFunction func, double leftBound, double rightBound, double precision)
        {
            int countIteration = GetFibonacciNumber(leftBound, rightBound, precision);
            double[] a = new double[countIteration];
            double[] b = new double[countIteration];
            double[] c = new double[countIteration];
            double[] y = new double[countIteration];
            double[] z = new double[countIteration];
            int k = 0;
            a[0] = leftBound;
            b[0] = rightBound;
            y[0] = a[0] + ((b[0] - a[0]) * fibonacciSequence[countIteration - 2]) / fibonacciSequence[countIteration];
            z[0] = a[0] + ((b[0] - a[0]) * fibonacciSequence[countIteration - 1]) / fibonacciSequence[countIteration];

            while (k != countIteration - 2)
            {
                if (func(y[k]) <= func(z[k]))
                {
                    a[k + 1] = a[k];
                    b[k + 1] = z[k];
                    y[k + 1] = a[k + 1] + ((b[k + 1] - a[k + 1]) * fibonacciSequence[countIteration - k - 3]) / fibonacciSequence[countIteration - k - 1];
                    z[k + 1] = y[k];
                }
                else
                {
                    a[k + 1] = y[k];
                    b[k + 1] = b[k];
                    y[k + 1] = z[k];
                    z[k + 1] = a[k + 1] + ((b[k + 1] - a[k + 1]) * fibonacciSequence[countIteration - k - 2]) / fibonacciSequence[countIteration - k - 1];
                }

                k++;
            }

            y[countIteration - 1] = y[countIteration - 2] = z[countIteration - 2];
            z[countIteration - 1] = y[countIteration - 1] + Epsilon;
            if (func(y[countIteration - 1]) <= func(z[countIteration - 1]))
            {
                a[countIteration - 1] = a[countIteration - 2];
                b[countIteration - 1] = z[countIteration - 1];
            }
            else
            {
                a[countIteration - 1] = y[countIteration - 1];
                b[countIteration - 1] = b[countIteration - 2];
            }

            return (b[countIteration - 1] + a[countIteration - 1]) / 2;
        }

        /// <summary>
        /// Нахождение безусловного максимума функции f(x) одной переменной.
        /// </summary>
        /// <param name="func">Функция f(x) одной переменной.</param>
        /// <param name="leftBound">Левая граница начального интервала неопределенности (a0).</param>
        /// <param name="rightBound">Правая граница начального интервала неопределенности (b0).</param>
        /// <param name="precision">Длина конечного интервала неопределенности (точность вычисления).</param>
        /// <returns>Безусловный максимум функции (x_max)</returns>
        public static double GetMaximum(OneVariableFunction func, double leftBound, double rightBound, double precision)
        {
            return GetMinimum(
                (double x) => { return -func(x); },
                leftBound,
                rightBound,
                precision);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Найдем число Фибоначчи, удовл. Fn >= (a0-b0)/precision
        /// </summary>
        /// <param name="leftBound">Левая граница начального интервала неопределенности (a0).</param>
        /// <param name="rightBound">Правая граница начального интервала неопределенности (b0).</param>
        /// <param name="precision">Длина конечного интервала неопределенности (точность вычисления).</param>
        /// <returns>
        /// номер числа в последовательности Фибоначчи
        /// </returns>
        private static int GetFibonacciNumber(double leftBound, double rightBound, double precision)
        {
            double tempF = (leftBound + rightBound) / precision;
            bool isFound = false;
            int i = 0;

            while (!isFound)
            {
                if (tempF <= fibonacciSequence[i])
                {
                    isFound = true;
                }
                else
                {
                    i++;
                }
            }

            return i;
        }

        #endregion
    }
}
