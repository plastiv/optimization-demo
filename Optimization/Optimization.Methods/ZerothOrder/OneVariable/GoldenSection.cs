//-----------------------------------------------------------------------
// <copyright file="GoldenSection.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace Optimization.Methods.ZerothOrder.OneVariable
{
    /// <summary>
    /// Оптимизация унимодальной функции одной переменной методом золотого сечения
    /// </summary>
    /// ----------------------------------------------------------------------------
    /// Метод относится к последовательным стратегиям. Задается начальный 
    /// интервал неопределенности и требуемая точность. Алгоритм уменьшения 
    /// интервала опирается на анализ значений функции в двух точках. В 
    /// качестве точек вычисления функции выбираются точки золотого сечения. Тогда с
    /// учетом свойств золотого сечения на каждой итерации, кроме первой, требуется
    /// только одно новое вычисление функции. Условия окончания процесса поиска
    /// стандартные: поиск заканчивается, когда длина текущего интервала 
    /// неопределенности оказывается меньше установленной величины.
    /// ----------------------------------------------------------------------------
    public static class GoldenSection
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

            double[] a = new double[count];
            double[] b = new double[count];
            double[] y = new double[count];
            double[] z = new double[count];

            a[0] = leftBound;
            b[0] = rightBound;
            y[0] = a[0] + ((b[0] - a[0]) * ((3 - System.Math.Sqrt(5)) / 2));
            z[0] = a[0] + b[0] - y[0];

            int k = 0; // Счетчик циклов

            while (b[k] - a[k] > precision)
            {
                if (func(y[k]) <= func(z[k]))
                {
                    a[k + 1] = a[k];
                    b[k + 1] = z[k];
                    y[k + 1] = a[k + 1] + b[k + 1] - y[k];
                    z[k + 1] = y[k];
                }
                else
                {
                    a[k + 1] = y[k];
                    b[k + 1] = b[k];
                    y[k + 1] = z[k];
                    z[k + 1] = a[k + 1] + b[k + 1] - z[k];
                }

                k++;
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
