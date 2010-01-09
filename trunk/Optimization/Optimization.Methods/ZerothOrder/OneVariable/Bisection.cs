//-----------------------------------------------------------------------
// <copyright file="Bisection.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace Optimization.Methods.ZerothOrder.OneVariable
{
    /// <summary>
    /// Оптимизация унимодальной функции одной переменной методом деления пополам
    /// </summary>
    /// ---------------------------------------------------------------------------------
    /// Метод относится к последовательным стратегиям и позволяет исключить
    /// из дальнейшего рассмотрения на каждой итерации в точности половину 
    /// текущего интервала неопределенности. Задается начальный интервал 
    /// неопределенности, а алгоритм уменьшения интервала, являясь, как и в общем 
    /// случае, "гарантирующим", основан на анализе величин функции в трех
    /// точках, равномерно распределенных на текущем интервале (делящих его на 
    /// четыре равные части). Условия окончания процесса поиска стандартные: поиск 
    /// заканчивается, когда длина текущего интервала неопределенности оказывается
    /// меньше установленной величины.
    /// ---------------------------------------------------------------------------------
    public static class Bisection
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
            System.Diagnostics.Debug.Assert(func != null, "func is unexeptedly equal to null");
            System.Diagnostics.Debug.Assert(leftBound < rightBound, "leftBound is unexeptedly less then rightBound");
            System.Diagnostics.Debug.Assert(precision > 0, "precision is unexeptedly less or equal to 0");

            // Количество вычислений функции для заданной точности
            int count = (int)System.Math.Ceiling((2 * System.Math.Log(precision) / System.Math.Log(0.5)));
            count++;

            double[] a = new double[count];
            double[] b = new double[count];
            double[] y = new double[count];
            double[] xavg = new double[count];
            double[] z = new double[count];
            int index = 0;

            a[0] = leftBound;
            b[0] = rightBound;
            xavg[0] = (a[0] + b[0]) / 2;

            while (b[index] - a[index] > precision)
            {
                y[index] = a[index] + ((b[index] - a[index]) / 4);
                z[index] = b[index] - ((b[index] - a[index]) / 4);

                if (func(y[index]) < func(xavg[index]))
                {
                    a[index + 1] = a[index];
                    b[index + 1] = xavg[index];
                    xavg[index + 1] = y[index];
                }
                else if (func(z[index]) < func(xavg[index]))
                {
                    a[index + 1] = xavg[index];
                    b[index + 1] = b[index];
                    xavg[index + 1] = z[index];
                }
                else
                {
                    a[index + 1] = y[index];
                    b[index + 1] = z[index];
                    xavg[index + 1] = xavg[index];
                }

                index++;
            }

            return xavg[index];
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
