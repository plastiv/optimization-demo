//-----------------------------------------------------------------------
// <copyright file="ModifedGoldenSection.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace OptimizationMethods.ZerothOrder.OneVariable
{
    /// <summary>
    /// Оптимизация унимодальной функции одной переменной методом модифицированного золотого сечения
    /// </summary>
    public static class ModifedGoldenSection
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
            int count = (int)System.Math.Ceiling((System.Math.Log(precision) / System.Math.Log(0.618)));
            count = (count * 2) + 1;

            double[] delta = new double[count];
            double[] y = new double[count];
            double[] z = new double[count];
            double[] a = new double[count];
            double[] b = new double[count];
            int k = 0;

            a[0] = leftBound;
            b[0] = rightBound;

            bool isCicl = true;
            while (isCicl)
            {
                delta[0] = b[0] - a[0];
                delta[1] = ((System.Math.Sqrt(5) - 1) / 2) * delta[0];
                delta[2] = delta[0] - delta[1];
                y[0] = a[0] + delta[2];
                z[0] = b[0] - delta[2];
                k = 1;

                bool isNum = true;
                while (isNum)
                {
                    if (func(y[k - 1]) <= func(z[k - 1]))
                    {
                        a[k] = a[k - 1];
                        b[k] = z[k - 1];
                        z[k] = y[k - 1];
                        delta[k + 2] = delta[k] - delta[k + 1];
                        y[k] = a[k] + delta[k + 2];
                        if (y[k] >= z[k])
                        {
                            a[0] = a[k];
                            b[0] = b[k];
                            isNum = false;
                            break;
                        }
                    }
                    else
                    {
                        a[k] = y[k - 1];
                        b[k] = b[k - 1];
                        y[k] = z[k - 1];
                        delta[k + 2] = delta[k] - delta[k + 1];
                        z[k + 1] = b[k] - delta[k + 2];

                        if (y[k] >= z[k])
                        {
                            a[0] = a[k];
                            b[0] = b[k];
                            isNum = false;
                            break;
                        }
                    }

                    if (delta[k] <= precision)
                    {
                        isNum = false;
                        isCicl = false;
                        break;
                    }
                    else if (delta[k] <= 0.8 * delta[k - 1])
                    {
                        k++;
                    }
                    else
                    {
                        a[0] = a[k];
                        b[0] = b[k];
                        isNum = false;
                        break;
                    }
                }
            }

            return (a[k] + b[k]) / 2;
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
