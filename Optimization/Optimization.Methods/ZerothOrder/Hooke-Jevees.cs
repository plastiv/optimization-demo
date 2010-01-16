//-----------------------------------------------------------------------
// <copyright file="Hooke-Jevees.cs" company="Home Corporation">
//     Copyright (c) Home Corporation 2009. All rights reserved.
// </copyright>
// <author>Sergii Pechenizkyi</author>
//-----------------------------------------------------------------------

namespace Optimization.Methods.ZerothOrder
{
    using System.Diagnostics;
    using Optimization.Methods;

    /// <summary>
    /// Нахождение безусловного минимума функции многих переменных методом Хука-Дживса
    /// </summary>
    internal class Hooke_Jevees
    {
        #region Private Fields
        /// <summary>
        /// Ссылка на функциональную зависимость.
        /// </summary>
        private readonly ManyVariable Function;

        /// <summary>
        /// Количество перменных
        /// </summary>
        protected internal readonly int Dimension;

        /// <summary>
        /// ускоряющий множитель lyamda > 0
        /// </summary>
        protected internal readonly double AccelerateCoefficient;

        /// <summary>
        /// коэффициент уменьшения шага аlfa > 1.
        /// </summary>
        protected internal readonly double CoefficientReduction;

        /// <summary>
        /// Значение шага по каждой из координат.
        /// </summary>
        protected internal double[] step;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Hooke_Jevees"/> class.
        /// </summary>
        /// <param name="inputFunc">The input function.</param>
        /// <param name="dimension">The dimension.</param>
        /// <param name="accelerateCoefficient">The accelerate coefficient.</param>
        /// <param name="coefficientReduction">The coefficient reduction.</param>
        /// <param name="step">The step value.</param>
        public Hooke_Jevees(ManyVariable inputFunc, int dimension, double accelerateCoefficient, double coefficientReduction, double[] step)
        {
            Debug.Assert(accelerateCoefficient > 0, "Accelerate coefficient lyamda is unexepectedly less or equal zero");
            Debug.Assert(coefficientReduction > 1, "Coefficient reduction alfa is unexepectedly less or equal 1");
            Debug.Assert(dimension > 1, "Dimension is unexepectedly less or equal 1");
            this.AccelerateCoefficient = accelerateCoefficient;
            this.CoefficientReduction = coefficientReduction;
            this.step = step;

            Debug.Assert(inputFunc != null, "Input function reference is unexepectedly null");
            this.Function = inputFunc;
            this.Dimension = dimension;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hooke_Jevees"/> class.
        /// </summary>
        /// <param name="inputFunc">The input function.</param>
        /// <param name="funcDimension">Количество переменных.</param>
        public Hooke_Jevees(ManyVariable inputFunc, int funcDimension)
            : this(inputFunc, funcDimension, 1.5, 4, null)
        {
            this.step = new double[funcDimension];
            for (int i = 0; i < funcDimension; i++)
            {
                this.step[i] = 1;
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
        internal double[] GetMinimum(double[] startPoint, double precision)
        {
            // Шаг 1. Задать начальную точку л:0
            // число е>0 для остановки алгоритма
            Debug.Assert(precision > 0, "Precision is unexepectedly less or equal zero");

            Point newBasis = new Point(startPoint);
            Point oldBasis = new Point(startPoint);

            while (true)
            {
                // Шаг 2. Осуществить исследующий поиск по выбранному координатному направлению (i)
                newBasis = this.ExploratarySearch(newBasis);

                // Проверить успешность исследующего поиска:
                if (this.GetFuncValue(newBasis) < this.GetFuncValue(oldBasis))
                {
                    // перейти к шагу 4;

                    // Сформируем х[k]
                    Point oldOldBasis = new Point(oldBasis.ToDouble());
                    oldBasis.SetEqual(newBasis);

                    // y[0] = x[k + 1] + AccelerateCoefficient * (x[k + 1] - x[k]);
                    newBasis = this.PatternSearch(oldOldBasis, oldBasis);

                    // перейти к шагу 2.
                    continue;
                }
                else
                {
                    // перейти к шагу 5.

                    // Шаг 5. Проверить условие окончания:
                    if (!this.AllStepsLessPrecision(precision))
                    {
                        for (int index = 0; index < this.Dimension; index++)
                        {
                            // Для значений шагов, больших точности
                            if (this.step[index] > precision)
                            {
                                // Уменьшить величину шага
                                this.step[index] /= this.CoefficientReduction;
                            }
                        }

                        newBasis.SetEqual(oldBasis);

                        // перейти к шагу 2.
                        continue;
                    }
                    else
                    {
                        // Значение всех шагов меньше точности
                        // Поиск закончен
                        return oldBasis.ToDouble();
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
        protected internal Point ExploratarySearch(Point point)
        {
            Point result = new Point(point.ToDouble());

            for (int i = 0; i < this.Dimension; i++)
            {
                if (this.GetFuncValue(this.GetPositiveProbe(result, i)) < this.GetFuncValue(result))
                {
                    // шаг считается удачным
                    result = this.GetPositiveProbe(result, i);
                }
                else
                {
                    // шаг неудачен, делаем шаг в противоположном направлении
                    if (this.GetFuncValue(this.GetNegativeProbe(result, i)) < this.GetFuncValue(result))
                    {
                        // шаг в противоположном направлении считается удачным
                        result = this.GetNegativeProbe(result, i);
                    }
                    else
                    {
                        // оба шага неудачны
                        // y[i + 1] = y[i];
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the func value.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>Значение функции в точке.</returns>
        protected internal double GetFuncValue(Point point)
        {
            return this.Function(point.ToDouble());
        }

        /// <summary>
        /// Пробный шаг в положительном направлении.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="i">Координата, по которой делаем шаг.</param>
        /// <returns>Новую точку.</returns>
        protected internal Point GetPositiveProbe(Point point, int i)
        {
            // TODO: Разобраться почему обязательно нужно чтобы был новый массив даблов в возращаемой Point, иначе данные сбиваются!
            // Потому что два раза вычисляется эта функция : сначала для проверки буля, а затем для присвоения, поэтому если буль не выдает тру , то значения все равно изменены.
            double[] solution = new Point(point.ToDouble()).ToDouble();
            solution[i] += this.step[i];
            return new Point(solution);
        }

        /// <summary>
        /// Пробный шаг в отрицательном направлении.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="i">Координата, по которой делаем шаг.</param>
        /// <returns>Новую точку.</returns>
        protected internal Point GetNegativeProbe(Point point, int i)
        {
            double[] solution = new Point(point.ToDouble()).ToDouble();
            solution[i] -= this.step[i];
            return new Point(solution);
        }

        /// <summary>
        /// Поиск по образцу. y[0] = x[k + 1] + AccelerateCoefficient * (x[k + 1] - x[k])
        /// </summary>
        /// <param name="oldBasis">The old basis.</param>
        /// <param name="basis">The basis.</param>
        /// <returns>Новую точку.</returns>
        protected internal Point PatternSearch(Point oldBasis, Point basis)
        {
            return basis + (this.AccelerateCoefficient * (basis - oldBasis));
        }

        /// <summary>
        /// Alls the steps less precision.
        /// </summary>
        /// <param name="precision">The precision.</param>
        /// <returns>True, если все</returns>
        protected internal bool AllStepsLessPrecision(double precision)
        {
            for (int index = 0; index < this.Dimension; index++)
            {
                if (this.step[index] > precision)
                {
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Structs

        #endregion
    }
}