//-----------------------------------------------------------------------
// <copyright file="Rosenbrock.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace Optimization.Methods.ZerothOrder
{
    using System.Diagnostics;

    /// <summary>
    /// Нахождение безусловного минимума функции многих переменных методом Розенброка
    /// </summary>
    internal class Rosenbrock
    {
        // TODO: доделать метод
        #region Private Fields
        /// <summary>
        /// Ссылка на функциональную зависимость.
        /// </summary>
        private readonly ManyVariable func;

        /// <summary>
        /// Параметры метода.
        /// </summary>
        private readonly MethodParams param;

        /// <summary>
        /// Значение шага по каждой из координат.
        /// </summary>
        private double[] step;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Rosenbrock"/> class.
        /// </summary>
        /// <param name="inputFunc">The input function.</param>
        /// <param name="inputParams">The input method parameters.</param>
        public Rosenbrock(ManyVariable inputFunc, MethodParams inputParams)
        {
            this.param = inputParams;

            Debug.Assert(inputFunc != null, "Input function reference is unexepectedly null");
            this.func = inputFunc;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rosenbrock"/> class.
        /// </summary>
        /// <param name="inputFunc">The input function.</param>
        /// <param name="funcDimension">Количество переменных.</param>
        public Rosenbrock(ManyVariable inputFunc, int funcDimension)
        {
            this.func = inputFunc;
            this.param.Alfa = 3; // рекомендации Розенброка
            this.param.Beta = -0.5; // рекомендации Розенброка
            this.param.Dimension = funcDimension;
            this.param.N = 5;
            this.step = new double[funcDimension];
            for (int i = 0; i < funcDimension; i++)
            {
                this.step[i] = 0.5;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the minimum.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="precision">The precision.</param>
        /// <returns>Вектор значений х, при котором функция достигает минимума.</returns>
        public double[] GetMinimum(double[] startPoint, double precision)
        {
            // Шаг 1. Задать начальную точку л:0
            // число е>0 для остановки алгоритма
            Debug.Assert(precision > 0, "Precision is unexepectedly less or equal zero");

            double[] newBasis = startPoint;
            double[] oldBasis = startPoint;

            while (true)
            {
                // Шаг 2. Осуществить исследующий поиск по выбранному координатному направлению (i)
                newBasis = this.ExploratarySearch(newBasis);

                // Проверить успешность исследующего поиска:
                if (this.func(newBasis) < this.func(oldBasis))
                {
                    // перейти к шагу 4;

                    // Шаг 4. Провести поиск по образцу. Положить xk+l = yn+l,
                    oldBasis = newBasis;

                    // перейти к шагу 2.
                    continue;
                }
                else
                {
                    // перейти к шагу 5.

                    // Шаг 5. Проверить условие окончания:
                    if (!this.AllStepsLessPrecision(precision))
                    {
                        // перейти к шагу 2.
                        continue;
                    }
                    else
                    {
                        // Значение всех шагов меньше точности
                        // Поиск закончен
                        return oldBasis;
                    }
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Пробный шаг.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>Новую точку.</returns>
        private double[] ExploratarySearch(double[] point)
        {
            for (int i = 0; i < this.param.Dimension; i++)
            {
                if (this.func(this.GetPositiveProbe(point, i)) < this.func(point))
                {
                    // шаг считается удачным
                    point = this.GetPositiveProbe(point, i);
                    this.step[i] *= this.param.Alfa;
                }
                else
                {
                    // шаг неудачен
                    // y[i + 1] = y[i];
                    this.step[i] *= this.param.Beta;
                }
            }

            return point;
        }

        /// <summary>
        /// Пробный шаг в положительном направлении.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="i">Координата, по которой делаем шаг.</param>
        /// <returns>Новую точку.</returns>
        private double[] GetPositiveProbe(double[] point, int i)
        {
            double[] solution = new double[this.param.Dimension];
            for (int j = 0; j < this.param.Dimension; j++)
            {
                solution[j] = point[j];
            }

            solution[i] += this.step[i];
            return solution;
        }

        /// <summary>
        /// Пробный шаг в отрицательном направлении.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="i">Координата, по которой делаем шаг.</param>
        /// <returns>Новую точку.</returns>
        private double[] GetNegativeProbe(double[] point, int i)
        {
            double[] solution = new double[this.param.Dimension];
            for (int j = 0; j < this.param.Dimension; j++)
            {
                solution[j] = point[j];
            }

            solution[i] -= this.step[i];
            return solution;
        }

        /// <summary>
        /// Alls the steps less precision.
        /// </summary>
        /// <param name="precision">The precision.</param>
        /// <returns>True, если все</returns>
        private bool AllStepsLessPrecision(double precision)
        {
            for (int index = 0; index < this.param.Dimension; index++)
            {
                if (System.Math.Abs(this.step[index]) > precision)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Structs
        /// <summary>
        /// Параметры метода.
        /// </summary>
        public struct MethodParams
        {
            /// <summary>
            /// начальные длина шага вдоль каждого из направлений поиска di,...,dn > 0
            /// </summary>
            public double[] Step;

            /// <summary>
            /// Количество перменных
            /// </summary>
            public int Dimension;

            /// <summary>
            /// ускоряющий множитель alfa > 1
            /// </summary>
            public double Alfa;

            /// <summary>
            /// коэффициент сжатия beta.
            /// </summary>
            public double Beta;

            /// <summary>
            /// Максимальное число неудавшихся серий шагов по всем направлениям на одной итерации.
            /// </summary>
            public double N;
        }
        #endregion
    }
}
