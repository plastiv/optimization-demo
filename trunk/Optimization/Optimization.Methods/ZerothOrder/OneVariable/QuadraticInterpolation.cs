//-----------------------------------------------------------------------
// <copyright file="QuadraticInterpolation.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace OptimizationMethods.ZerothOrder.OneVariable
{
    /// <summary>
    /// Оптимизация унимодальной функции одной переменной методом квадратичной интерполяции (метод Пауэлла [Powell M.J.D.], метод парабол)
    /// </summary>
    public static class QuadraticInterpolation
    {
        #region Стратегия поиска
        // --------------------------------------------------------------------------
        // Метод квадратичной интерполяции (метод Пауэлла [Powell M. J. D.]) относится
        // к последовательным стратегиям. Задается начальная точка и с помощью
        // пробного шага находятся три точки так, чтобы они были как можно ближе к 
        // искомой точке минимума. В полученных точках вычисляются значения функции.
        // Затем строится интерполяционный полином второй степени, проходящий через
        // имеющиеся три точки. В качестве приближения точки минимума берется точка
        // минимума полинома. Процесс поиска заканчивается, когда полученная точка 
        // отличается от наилучшей из трех опорных точек не более чем на заданную 
        // величину.
        // --------------------------------------------------------------------------
        #endregion

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
            double x1, x2, xminInterp, x4;
            double f1, f2, dx1, dx2;

            x1 = leftBound;
            x2 = (rightBound + leftBound) / 2;
            x4 = rightBound;
            xminInterp = (x4 + x2) / 2;

            while (System.Math.Abs(x2 - xminInterp) > precision)
            {
                if (xminInterp < x2)
                {
                    double tmp;
                    tmp = x2;
                    x2 = xminInterp;
                    xminInterp = tmp;
                }

                if (func(x2) > func(xminInterp))
                {
                    x1 = x2;
                    x2 = xminInterp;
                }
                else
                {
                    x4 = xminInterp;
                }

                f1 = func(x2) - func(x1);
                f2 = func(x2) - func(x4);
                dx1 = x2 - x1;
                dx2 = x4 - x2;
                xminInterp = x2 - ((dx1 * dx1 * f2 - dx2 * dx2 * f1) / (2 * (dx1 * f2 - dx2 * f1)));

                if (!(x1 < xminInterp && xminInterp < x4))
                {
                    x2 = (x1 + x4) / 2;
                    xminInterp = (x2 + x4) / 2;
                }
            }

            return xminInterp;
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
