//-----------------------------------------------------------------------
// <copyright file="Dichotomy.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace Optimization.Methods.ZerothOrder.OneVariable
{
    /// <summary>
    /// Оптимизация унимодальной функции одной переменной методом дихотомии (последовательное деление на две части)
    /// </summary>
    /// /// ----------------------------------------------------------------------------
    /// Метод относится к последовательным стратегиям. Задается начальный интервал 
    /// неопределенности и требуемая точность. Алгоритм опирается на анализ значений
    /// функции в двух точках. Для их нахождения текущий интервал неопределенности
    /// делится пополам и в обе стороны от середины откладывается по eps/2 ,
    /// где eps - малое положительное число. Условия окончания процесса поиска 
    /// стандартные: поиск заканчивается, когда длина текущего интервала 
    /// неопределенности оказывается меньше установленной величины.
    /// ----------------------------------------------------------------------------
    public static class Dichotomy
    {
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
            // Количество вычислений функции для заданной точности
            int count = (int)System.Math.Ceiling((2 * System.Math.Log(precision) / System.Math.Log(0.5)));
            count++;

            // Малое положительное число
            double epsilon = precision / 5;
            double[] a = new double[count];
            double[] y = new double[count];
            double[] z = new double[count];
            double[] b = new double[count];
            int index = 0;

            a[0] = leftBound;
            b[0] = rightBound;

            while (b[index] - a[index] > precision)
            {
                y[index] = (a[index] + b[index] - epsilon) / 2;
                z[index] = (a[index] + b[index] + epsilon) / 2;

                if (func(y[index]) < func(z[index]))
                {
                    a[index + 1] = a[index];
                    b[index + 1] = z[index];
                }
                else
                {
                    a[index + 1] = y[index];
                    b[index + 1] = b[index];
                }

                index++;
            }

            return (a[index] + b[index]) / 2;
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
    }
}
