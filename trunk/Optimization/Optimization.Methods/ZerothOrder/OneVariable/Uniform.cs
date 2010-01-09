//-----------------------------------------------------------------------
// <copyright file="Uniform.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace Optimization.Methods.ZerothOrder.OneVariable
{
    /// <summary>
    /// Оптимизация унимодальной функции одной переменной методом равномерного поиска
    /// http://ru.wikipedia.org/wiki/Метод_перебора
    /// </summary>
    /// ----------------------------------------------------------------------------------
    /// Метод относится к пассивным стратегиям. Задается начальный интервал
    /// неопределенности L0=[a0,b0] и количество вычислений функции N. 
    /// Вычисления производятся в N равноотстоящих друг от друга точках (при этом интервал
    /// Lo делится на N + 1 равных интервалов). Путем сравнения величин f(xi),
    /// i = 1, 2, ..., N находится точка xk, в которой значение функции наименьшее.
    /// Искомая точка минимума х* считается заключенной в интервале [хk-1,хk+1].
    /// ----------------------------------------------------------------------------------
    public static class Uniform
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
            int count = (int)((System.Math.Abs(leftBound - rightBound) / precision) + 1);
            double minimum = func(leftBound);
            int minIndex = 0;

            for (int i = 1; i < count; i++)
            {
                if (func((leftBound + i * precision)) < minimum)
                {
                    minimum = func((leftBound + i * precision));
                    minIndex = i;
                }
            }

            return leftBound + (minIndex * precision);
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
