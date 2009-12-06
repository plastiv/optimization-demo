//-----------------------------------------------------------------------
// <copyright file="ModifedUniform.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace OptimizationMethods.ZerothOrder.OneVariable
{
    /// <summary>
    /// Оптимизация унимодальной функции одной переменной методом модифицированного равномерного поиска
    /// http://ru.wikipedia.org/wiki/Метод_перебора#Модификация
    /// </summary>
    public static class ModifedUniform
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

            double tempVar = (rightBound - leftBound) / ((count / 2) + 1);
            double epsilon = ((2 * (rightBound - leftBound)) / (count + 1)) - tempVar;

            if (count % 2 != 0)
            {
                count++;
            }

            count /= 2;

            double minimumY = func(leftBound);
            double minimumX = 0;

            for (int i = 2, j = 1; i < count && j < count; i += 2, j += 2)
            {
                if (func(leftBound + tempVar * i) < minimumY)
                {
                    minimumX = leftBound + tempVar * i;
                    minimumY = func(minimumX);
                }

                if (func(leftBound + tempVar * i - epsilon) < minimumY)
                {
                    minimumX = leftBound + (tempVar * i) - epsilon;
                    minimumY = func(minimumX);
                }
            }

            return minimumX;
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
