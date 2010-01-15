//-----------------------------------------------------------------------
// <copyright file="ManyVariable.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace Optimization.Methods
{
    using Optimization.Methods.FirstOrder;
    using Optimization.Methods.ZerothOrder;

    /// <summary>
    /// Ссылка на функциональную зависимость f(x1,х2,...,хn).
    /// </summary>
    /// <param name="x">Вектор значений переменных x1,х2,...,хn.</param>
    /// <returns>Значение функции у=f(x1,х2,...,хn).</returns>
    public delegate double ManyVariable(double[] x);

    /// <summary>
    /// Ссылка на функциональную зависимость df/dxi, где xi = x1,х2,...,хn.
    /// </summary>
    /// <param name="x">Вектор значений переменных x1,х2,...,хn.</param>
    /// <returns>Значение градиента у=df/dxi, где xi = x1,х2,...,хn.</returns>
    public delegate double[] Gradient(double[] x);

    /// <summary>
    /// Нахождение безусловного минимума функции многих переменных
    /// </summary>
    public class Minimum
    {
        /// <summary>
        /// Малое число для остановки алгоритма.
        /// </summary>
        private const double Precision = 0.01;

        /// <summary>
        /// Gradients the descent.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="dimension">The dimension.</param>
        /// <param name="startingPoint">The starting point.</param>
        /// <returns>Минимум функции.</returns>
        public static double[] GradientDescent(ManyVariable function, int dimension, double[] startingPoint)
        {
            GradientDescent gd = new GradientDescent(function, dimension);
            return gd.GetMinimum(startingPoint);
        }

        /// <summary>
        /// Gradients the descent.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="dimension">The dimension.</param>
        /// <param name="startingPoint">The starting point.</param>
        /// <returns>Минимум функции со значениями промежуточных точек.</returns>
        public static double[][] GradientDescentExtended(ManyVariable function, int dimension, double[] startingPoint)
        {
            GradientDescentExtended gde = new GradientDescentExtended(function, dimension);
            return gde.GetMinimum(startingPoint);
        }

        /// <summary>
        /// Нахождение безусловного минимума функции многих переменных методом деформируемого многогранника.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="dimension">The dimension.</param>
        /// <param name="startingPoint">The starting point.</param>
        /// <returns>Минимум функции.</returns>
        public static double[] DeformablePolyhedron(ManyVariable function, int dimension, double[] startingPoint)
        {
            DeformablePolyhedron dp = new DeformablePolyhedron(function, dimension);
            return dp.GetMinimum(startingPoint, Precision);
        }

        /// <summary>
        /// Нахождение безусловного минимума функции многих переменных методом деформируемого многогранника.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="dimension">The dimension.</param>
        /// <param name="startingPoint">The starting point.</param>
        /// <returns>Минимум функции.</returns>
        public static double[][][] DeformablePolyhedronExtended(ManyVariable function, int dimension, double[] startingPoint)
        {
            DeformablePolyhedron dp = new DeformablePolyhedron(function, dimension);
            return dp.GetExtendedMinimum(startingPoint, Precision);
        }

        /// <summary>
        /// Нахождение безусловного минимума функции многих переменных методом Хука-Дживса.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="dimension">The dimension.</param>
        /// <param name="startingPoint">The starting point.</param>
        /// <returns>Минимум функции.</returns>
        public static double[] HookeJevees(ManyVariable function, int dimension, double[] startingPoint)
        {
            Hooke_Jevees hj = new Hooke_Jevees(function, dimension);
            return hj.GetMinimum(startingPoint, Precision);
        }

        /// <summary>
        /// Нахождение безусловного минимума функции многих переменных методом Хука-Дживса.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="dimension">The dimension.</param>
        /// <param name="startingPoint">The starting point.</param>
        /// <returns>Минимум функции.</returns>
        public static double[][] HookeJeveesExtended(ManyVariable function, int dimension, double[] startingPoint)
        {
            Hooke_Jevees hj = new Hooke_Jevees(function, dimension);
            return hj.GetMinimumExtended(startingPoint, Precision);
        }

        /// <summary>
        /// Rosenbrocks the specified function.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="dimension">The dimension.</param>
        /// <param name="startingPoint">The starting point.</param>
        /// <returns>Минимум функции.</returns>
        public static double[] Rosenbrock(ManyVariable function, int dimension, double[] startingPoint)
        {
            Rosenbrock rb = new Rosenbrock(function, dimension);
            return rb.GetMinimum(startingPoint, Precision);
        }

        /// <summary>
        /// Нахождение безусловного минимума функции многих переменных методом случайного поиска.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="dimension">The dimension.</param>
        /// <param name="startingPoint">The starting point.</param>
        /// <returns>Минимум функции.</returns>
        public static double[] Random(ManyVariable function, int dimension, double[] startingPoint)
        {
            Random rn = new Random(function, dimension);
            return rn.GetMinimum(startingPoint, Precision);
        }
    }
}